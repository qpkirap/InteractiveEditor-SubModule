﻿using System.Linq;
using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;
using UnityEngine;
using VContainer;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceSaveDialogueNodeExecutor<TNode> : BaseAnswerChoiceSaveDialogueNodeExecutor<AnswerChoiceSave<TNode>, TNode>
        where TNode : BaseNode
    {
    }
    
    public abstract class BaseAnswerChoiceSaveDialogueNodeExecutor<TSave, TNode> : INodeExecute<TNode>
        where TSave : AnswerChoiceSave<TNode>
        where TNode : BaseNode
    {
        [Inject] private readonly SaveManager saveManager;
        
        public BaseNode GetNext(TNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(TNode baseNode)
        {
            if (saveManager.NodeSaveServices.GetSaveItem(baseNode.Id) is not TSave saveItem) return ExecuteResult.SuccessState;

            saveItem.UpdateData();
            
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(TNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(TNode baseNode)
        {
        }
    }
}