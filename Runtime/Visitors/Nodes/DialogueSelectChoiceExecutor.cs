using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceExecutor : INodeExecute<SelectChoiceDialogueNode>
    {
        private static LazyInject<IRouter> router;
        
        private List<LocalizedString> answersCache;
        private SelectChoiceDialogueNode node;
        
        private int selectedIndex = -1;
        private bool isOpenCanvas;

        public IReadOnlyList<LocalizedString> GetAnswers()
        {
            if (answersCache is { Count: > 0 }) return answersCache;
            
            if (node == null) return null;

            var answerChoice = node.ChildrenNodes.Cast<AnswerChoiceDialogueNode>();
            
            answersCache = answerChoice.Select(x => x.AnswerText).ToList();

            return answersCache;
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
            
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return ExecuteResult.SuccessState;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.baseDialogue);
                
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
            answersCache.Clear();
            
            selectedIndex = -1;
            isOpenCanvas = false;
        }
    }
}