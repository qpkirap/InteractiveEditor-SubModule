using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueAnswerExecutor : INodeExecute<AnswerChoiceDialogueNode>
    {
        public BaseNode GetNext(AnswerChoiceDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(AnswerChoiceDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(AnswerChoiceDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(AnswerChoiceDialogueNode baseNode)
        {
        }
    }
}