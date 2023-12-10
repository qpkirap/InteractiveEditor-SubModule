using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class ActionExecutor : INodeExecute<BaseActionNode>
    {
        private IUniTaskAsyncEnumerable<ActionTaskComponent> collection;
        
        private readonly CancellationTokenHandler executeTokenHandler = new();
        private readonly CancellationTokenHandler cancelTokenHandler = new();
        private UniTask? executeTask;
        private UniTask? cancelTask;
        
        public ExecuteResult Execute(BaseActionNode baseNode)
        {
            if (baseNode.Tasks == null || !baseNode.Tasks.Any()) return ExecuteResult.SuccessState;

            cancelTokenHandler?.CancelOperation();
            
            if (executeTask is { Status: UniTaskStatus.Succeeded })
            {
                return ExecuteResult.SuccessState;
            }

            if (executeTask == null || executeTask.HasValue && executeTask.Value.Status != UniTaskStatus.Pending)
            {
                executeTask = ExecuteOrCancelAsync(baseNode);

                return ExecuteResult.RunningState;
            }
            else if (executeTask is { Status: UniTaskStatus.Pending })
            {
                return ExecuteResult.RunningState;
            }

            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(BaseActionNode baseNode)
        {
            if (baseNode.Tasks == null || !baseNode.Tasks.Any()) return ExecuteResult.SuccessState;

            executeTokenHandler?.CancelOperation();
            
            if (cancelTask is { Status: UniTaskStatus.Succeeded })
            {
                return ExecuteResult.SuccessState;
            }

            if (cancelTask == null || cancelTask.HasValue && cancelTask.Value.Status != UniTaskStatus.Pending)
            {
                cancelTask = ExecuteOrCancelAsync(baseNode, true);

                return ExecuteResult.RunningState;
            }
            else if (cancelTask is { Status: UniTaskStatus.Pending })
            {
                return ExecuteResult.RunningState;
            }

            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(BaseActionNode baseNode)
        {
            collection = null;
            executeTask = null;
            cancelTask = null;
        }

        public BaseNode GetNext(BaseActionNode baseNode)
        {
            return baseNode.ChildrenNodes?.FirstOrDefault();
        }
        
        private async UniTask ExecuteOrCancelAsync(BaseActionNode node, bool isCancel = false)
        {
            collection ??= node.Tasks.ToUniTaskAsyncEnumerable();

            try
            {
                await collection.ForEachAwaitAsync(async item =>
                {
                    if (!isCancel) await item.Execute(executeTokenHandler.Token);
                    else await item.Undo(executeTokenHandler.Token);

                    executeTokenHandler.Token.ThrowIfCancellationRequested();

                }, executeTokenHandler.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Operation canceled.");
            }
        }
    }
}