﻿﻿﻿using System.Linq;
using Module.InteractiveEditor.Configs;
using Managers.Router;

namespace Module.InteractiveEditor.Runtime
{
    public class ConditionsNodeExecutor : INodeExecutor<ConditionsNode, UICanvas>
    {
        private ConditionsNode currentNode;
        
        public BaseNode GetNext(ConditionsNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) 
                return null;
            
            // Evaluate conditions here and return appropriate node
            // For now, return random child node
            return baseNode.ChildrenNodes[UnityEngine.Random.Range(0, baseNode.ChildrenNodes.Count)];
        }

        public ExecuteResult Execute(ConditionsNode baseNode)
        {
            currentNode = baseNode;
            
            // Evaluate all conditions
            bool allConditionsMet = EvaluateConditions(baseNode);
            
            return allConditionsMet ? ExecuteResult.SuccessState : ExecuteResult.NoneState;
        }

        public ExecuteResult Cancel(ConditionsNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(ConditionsNode baseNode)
        {
            currentNode = null;
        }
        
        public void InitializeView(UICanvas uiCanvas)
        {
            // Conditions typically don't have a visual representation
            // This could be used for debug UI or condition visualization
        }
        
        public void ResetView()
        {
            // No view state to reset for conditions
        }
        
        private bool EvaluateConditions(ConditionsNode node)
        {
            // This would contain the logic to evaluate all conditions
            // For now, return true as placeholder
            return true;
        }
    }
}