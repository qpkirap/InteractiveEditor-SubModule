﻿﻿﻿using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves.UI.Story;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceConditionsExecutor : INodeExecutor<SelectChoiceDialogueConditionsNode, DialogueSelectChoiceConditionsCanvas>
    {
        private static LazyInject<IRouter> router = new();
        private readonly CompositeDisposable disp = new();
        
        private readonly Dictionary<int, (IEnumerable<ICondition> conditions, BaseNode node)> answersConditions = new();
        private readonly List<LocalizedString> answersCache = new();

#if !UNITY_WEBGL
        private AddressableSprite background;
#endif
        private ImageData imageDataCache;
        private SelectChoiceDialogueConditionsNode node;
        
        private bool isOpenCanvas;
        
        public int SelectedIndex { get; private set; }= -1;
        
        public void SetSelectedIndex(int index)
        {
            SelectedIndex = index;
        }

        public IReadOnlyList<LocalizedString> GetAnswers()
        {
            if (answersCache is { Count: > 0 }) return answersCache;
            
            if (node == null) return null;

            var answerChoice = node.ChildrenNodes.Cast<AnswerChoiceConditionsDialogueNode>().ToArray();
            answersConditions.Clear();
            
            for (var i = 0; i < answerChoice.Length; i++)
            {
                var conditions = answerChoice[i].Conditions.Select(x => x.GetCondition());

                answersConditions.Add(i, (conditions, answerChoice[i]));
            }
            
            foreach (var (index, (conditions, node)) in answersConditions)
            {
                var test = true;
                
                foreach (var condition in conditions)
                {
                    if (!condition.IsTrue(node))
                    {
                        test = false;
                        break;
                    }
                }
                
                if (!test) continue;
                
                var answer = answerChoice[index].AnswerText;
                answersCache.Add(answer);
            }

            return answersCache;
        }

#if !UNITY_WEBGL
        public AddressableSprite GetBackground()
        {
            var data = GetImageData();

            if (data == null) return null;
            
            background ??= data.Image;
            
            return background;
        }
#endif
        
        public Sprite GetSprite()
        {
            return GetImageData()?.ImageSprite;
        }
        
        private ImageData GetImageData()
        {
            imageDataCache ??= node.GetRandomData;

            return imageDataCache;
        }
        
        public IReadOnlyList<CensureData> GetCensures()
        {
            return GetImageData()?.Censures;
        }

        public LocalizedString GetText()
        {
            return node.Dialogue;
        }

        public BaseNode GetNext(SelectChoiceDialogueConditionsNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return null;
            
            if (SelectedIndex >= 0)
            {
                if (answersConditions.TryGetValue(SelectedIndex, out var infos))
                {
                    return infos.node;
                }
            }

            return null;
        }

        public ExecuteResult Execute(SelectChoiceDialogueConditionsNode baseNode)
        {
            node ??= baseNode;
            
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return ExecuteResult.SuccessState;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.dialogueSelectChoiceConditions, routArgs: new (string, object)[]
                {
                    (INodeExecutor.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }

            var state = SelectedIndex < 0 ? ExecuteResult.RunningState : ExecuteResult.SuccessState;
            
            return state;
        }

        public ExecuteResult Cancel(SelectChoiceDialogueConditionsNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SelectChoiceDialogueConditionsNode baseNode)
        {
            node = null;
            answersCache?.Clear();
            answersConditions?.Clear();
            
            SelectedIndex = -1;
            isOpenCanvas = false;

#if !UNITY_WEBGL
            background = null;
#endif
            imageDataCache = null;
            disp.Clear();
        }
        
        // View execution methods
        public void InitializeView(DialogueSelectChoiceConditionsCanvas uiCanvas)
        {
            if (uiCanvas == null) return;
            
            // Set up the dialogue UI with current node data
            UpdateDialogueUI(uiCanvas);
            
            // Set up choices and handle selection
            var answers = GetAnswers();
            if (answers != null && answers.Count > 0)
            {
                uiCanvas.SetChoices(answers, out var choiceObservable);
                
                if (choiceObservable != null)
                {
                    choiceObservable.Subscribe(index => SetSelectedIndex(index)).AddTo(disp);
                }
            }
        }
        
        public void ResetView()
        {
            // Reset any view-specific state if needed
            disp.Clear();
        }
        
        private void UpdateDialogueUI(DialogueSelectChoiceConditionsCanvas uiCanvas)
        {
            if (node == null) return;
            
            // Set text
            uiCanvas.SetText(GetText());
            
            // Set background image
#if !UNITY_WEBGL
            var background = GetBackground();
            if (background != null)
            {
                uiCanvas.SetImage(background);
            }
            else
#endif
            {
                var sprite = GetSprite();
                if (sprite != null)
                {
                    uiCanvas.SetImage(sprite);
                }
            }
            
            // Set censures
            var censures = GetCensures();
            if (censures != null)
            {
                uiCanvas.SetCensure(censures);
            }
        }
    }
}