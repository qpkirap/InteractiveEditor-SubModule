using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.InteractiveEditor.Runtime;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class BaseActionNode : BaseNode
    {
        [SerializeField] private List<ActionTask> tasks;

        private IUniTaskAsyncEnumerable<ActionTask> collection;
        
        private readonly CancellationTokenHandler executeTokenHandler = new();
        private readonly CancellationTokenHandler cancelTokenHandler = new();
        private UniTask? executeTask;
        private UniTask? cancelTask;
        
        public override NodeType NodeType => NodeType.Action;

        #region Editor

        public const string TasksKey = nameof(tasks);

        #endregion
        protected override ExecuteResult ExecuteTask()
        {
            if (tasks == null) return ExecuteResult.SuccessState;
            
            cancelTokenHandler.CancelOperation();
            
            if (executeTask == null || executeTask.HasValue && executeTask.Value.Status != UniTaskStatus.Pending)
            {
                executeTask = Execute();
                    
                return ExecuteResult.RunningState;
            }
            else if (executeTask is { Status: UniTaskStatus.Pending })
            {
                return ExecuteResult.RunningState;
            }

            return ExecuteResult.SuccessState;
        }

        protected override ExecuteResult CancelTask()
        {
            if (tasks == null) return ExecuteResult.SuccessState;
            
            executeTokenHandler?.CancelOperation();

            if (cancelTask == null || cancelTask.HasValue && cancelTask.Value.Status != UniTaskStatus.Pending)
            {
                cancelTask = Execute(true);
                    
                return ExecuteResult.RunningState;
            }
            else if (cancelTask is { Status: UniTaskStatus.Pending })
            {
                return ExecuteResult.RunningState;
            }
            
            return ExecuteResult.SuccessState;
        }

        private async UniTask Execute(bool isCancel = false)
        {
            collection ??= tasks.ToUniTaskAsyncEnumerable();
            
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