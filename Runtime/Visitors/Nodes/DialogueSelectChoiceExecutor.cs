using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceExecutor : INodeExecute<SelectChoiceDialogueNode>
    {
        private static LazyInject<IRouter> router;
        private int selectedIndex = -1;
        private bool isOpenCanvas;
        
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
            selectedIndex = -1;
            isOpenCanvas = false;
        }
    }
}