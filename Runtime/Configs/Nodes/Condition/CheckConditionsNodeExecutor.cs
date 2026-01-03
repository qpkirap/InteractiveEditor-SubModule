using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using VContainer;

namespace Module.InteractiveEditor.Runtime
{
    public class CheckConditionsNodeExecutor : INodeExecute<CheckConditionsNode>
    {
        [Inject] private readonly IObjectResolver resolver;
        
        private Dictionary<int, List<ICondition>> conditionsByIndex = new();
        private bool isCreateCondition = false;
        private int selectIndexNode = -1;
        
        public BaseNode GetNext(CheckConditionsNode baseNode)
        {
            if (baseNode.ChildrenNodes == null 
                || !baseNode.ChildrenNodes.Any() 
                || selectIndexNode < 0
                || selectIndexNode >= baseNode.ChildrenNodes.Count) return null;

            return baseNode.ChildrenNodes[selectIndexNode];
        }

        public ExecuteResult Execute(CheckConditionsNode baseNode)
        {
            if (!isCreateCondition)
            {
                isCreateCondition = true;

                for (var i = 0; i < baseNode.ChildrenNodes.Count; i++)
                {
                    var node = baseNode.ChildrenNodes[i];

                    if (node is ConditionsNode conditionNode)
                    {
                        var conditions = conditionNode.Conditions
                            .Select(x =>
                            {
                                var condition = x.GetCondition();
                                resolver.Inject(condition);
                                return condition;
                            });
                        conditionsByIndex.Add(i, conditions.ToList());
                    }
                }
            }

            if (selectIndexNode < 0)
            {
                foreach (var (index, conditions) in conditionsByIndex)
                {
                    var test = true;
                
                    foreach (var condition in conditions)
                    {
                        if (!condition.IsTrue(baseNode))
                        {
                            test = false;
                            break;
                        }
                    }
                    
                    if (test && index != -1)
                    {
                        selectIndexNode = index;
                        break;
                    }
                }
            }

            return selectIndexNode switch
            {
                < 0 => ExecuteResult.RunningState,
                >= 0 => ExecuteResult.SuccessState
            };
        }

        public ExecuteResult Cancel(CheckConditionsNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(CheckConditionsNode baseNode)
        {
            isCreateCondition = false;
            conditionsByIndex.Clear();
            selectIndexNode = -1;
        }
    }
}