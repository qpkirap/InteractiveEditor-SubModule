using System.Linq;
using DepedencyInjection;
using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceSaveDialogueNodeExecutor : INodeExecute<AnswerChoiceSaveDialogueNode>
    {
        private static LazyInject<SaveManager> saveManager = new();
        
        public BaseNode GetNext(AnswerChoiceSaveDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(AnswerChoiceSaveDialogueNode baseNode)
        {
            if (saveManager.Value.NodeSaveServices.GetSaveItem(baseNode.Id) is not AnswerChoiceSave saveItem) return ExecuteResult.SuccessState;

            saveItem.UpdateData();
            
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(AnswerChoiceSaveDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(AnswerChoiceSaveDialogueNode baseNode)
        {
        }
    }
}