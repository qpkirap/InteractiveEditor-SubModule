using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceSaveExecutor : INodeExecute<SelectChoiceDialogueSaveNode>
    {
        private static LazyInject<IRouter> router = new();
        
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

        public BaseNode GetNext(SelectChoiceDialogueSaveNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return null;
            
            if (SelectedIndex >= 0)
            {
                return baseNode.ChildrenNodes[Mathf.Clamp(SelectedIndex, 0, baseNode.ChildrenNodes.Count - 1)];
            }

            return null;
        }

        public ExecuteResult Execute(SelectChoiceDialogueSaveNode baseNode)
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

            return SelectedIndex < 0 ? ExecuteResult.RunningState : ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(SelectChoiceDialogueSaveNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SelectChoiceDialogueSaveNode baseNode)
        {
            node = null;
            answersCache.Clear();
            
            SelectedIndex = -1;
            isOpenCanvas = false;
        }
    }
}