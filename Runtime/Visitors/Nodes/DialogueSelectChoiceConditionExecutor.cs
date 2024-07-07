using System;
using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceConditionExecutor : INodeExecute<SelectChoiceDialogueConditionNode>
    {
        private static LazyInject<IRouter> router = new();
        private static LazyInject<SaveManager> saveManager = new();
        
        private AddressableSprite background;
        private ImageData imageDataCache;
        private SelectChoiceDialogueNode node;
        
        private List<LocalizedString> answersCache;
        
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

            var answerChoice = node.ChildrenNodes.Cast<AnswerChoiceDialogueNode>();
            
            answersCache = answerChoice.Select(x => x.AnswerText).ToList();

            return answersCache;
        }
        
        public AddressableSprite GetBackground()
        {
            var data = GetImageData();

            if (data == null) return null;
            
            background ??= data.Image;
            
            return background;
        }
        
        private ImageData GetImageData()
        {
            imageDataCache ??= node.RandomImage;

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

        public BaseNode GetNext(SelectChoiceDialogueConditionNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return null;
            
            if (SelectedIndex >= 0)
            {
                return baseNode.ChildrenNodes[Mathf.Clamp(SelectedIndex, 0, baseNode.ChildrenNodes.Count - 1)];
            }

            return null;
        }

        public ExecuteResult Execute(SelectChoiceDialogueConditionNode baseNode)
        {
            node ??= baseNode;
            
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return ExecuteResult.SuccessState;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.dialogueSelectChoice, routArgs: new (string, object)[]
                {
                    (INodeExecute.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }

            var state = SelectedIndex < 0 ? ExecuteResult.RunningState : ExecuteResult.SuccessState;

            if (state == ExecuteResult.SuccessState)
            {
                var save = saveManager.Value.NodeSaveServices.GetSaveItem(baseNode.Id);

                if (save != default)
                {
                    var typeSave = node.GetSaveItemType();
                    //TODO нужно как то доработать
                }
            }
            
            return state;
        }

        public ExecuteResult Cancel(SelectChoiceDialogueConditionNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SelectChoiceDialogueConditionNode baseNode)
        {
            node = null;
            answersCache.Clear();
            
            SelectedIndex = -1;
            isOpenCanvas = false;
        }
    }
}