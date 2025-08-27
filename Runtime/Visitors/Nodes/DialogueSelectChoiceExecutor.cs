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
    public class DialogueSelectChoiceExecutor : INodeExecutor<SelectChoiceDialogueNode, DialogueSelectChoiceCanvas>
    {
        private static LazyInject<IRouter> router = new();
        private readonly CompositeDisposable disp = new();
        
        private AddressableSprite background;
        private ImageData imageDataCache;
        private SelectChoiceDialogueNode node;
        
        private List<LocalizedString> answersCache;
        
        private int selectedIndex = -1;
        private bool isOpenCanvas;
        
        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
        }

        public IReadOnlyList<LocalizedString> GetAnswers()
        {
            if (answersCache is { Count: > 0 }) return answersCache;
            
            if (node == null) return null;

            var answerChoice = node.ChildrenNodes.Cast<AnswerChoiceDialogueNode>();
            
            answersCache = answerChoice.Select(x => x.AnswerText).ToList();

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

        public BaseNode GetNext(SelectChoiceDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return null;
            
            if (selectedIndex >= 0)
            {
                return baseNode.ChildrenNodes[Mathf.Clamp(selectedIndex, 0, baseNode.ChildrenNodes.Count - 1)];
            }

            return null;
        }

        public ExecuteResult Execute(SelectChoiceDialogueNode baseNode)
        {
            node ??= baseNode;
            
            if (baseNode.ChildrenNodes == null) return ExecuteResult.SuccessState;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.dialogueSelectChoice, routArgs: new (string, object)[]
                {
                    (INodeExecutor.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }

            return selectedIndex < 0 ? ExecuteResult.RunningState : ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(SelectChoiceDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SelectChoiceDialogueNode baseNode)
        {
            node = null;
            answersCache?.Clear();
            
            selectedIndex = -1;
            isOpenCanvas = false;
            
            background = null;
            imageDataCache = null;
            disp.Clear();
        }
        
        // View execution methods
        public void InitializeView(DialogueSelectChoiceCanvas uiCanvas)
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
        
        private void UpdateDialogueUI(DialogueSelectChoiceCanvas uiCanvas)
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