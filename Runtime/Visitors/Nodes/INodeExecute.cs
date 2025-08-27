﻿﻿﻿﻿using System;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine;

// Type alias for backward compatibility
using INodeExecute = Module.InteractiveEditor.Runtime.INodeExecutor;

namespace Module.InteractiveEditor.Runtime
{
    public interface INodeExecutor
    {
        public static RoutArgKey NodeExecutorKey = new("NodeExecutorKey");
        
        ExecuteResult Execute(BaseNode baseNode);
        ExecuteResult Cancel(BaseNode baseNode);
        BaseNode GetNext(BaseNode baseNode);
        void ResetExecutor(BaseNode baseNode);
        
        // View execution methods
        void InitializeView(UICanvas uiCanvas);
        void ResetView();
    }
    
    public interface INodeExecutor<in TNode, in TCanvas> : INodeExecutor
        where TNode : BaseNode
        where TCanvas : UICanvas
    {
        // Logic execution methods
        BaseNode GetNext(TNode baseNode);
        ExecuteResult Execute(TNode baseNode);
        ExecuteResult Cancel(TNode baseNode);
        void ResetExecutor(TNode baseNode);
        
        // View execution methods
        void InitializeView(TCanvas uiCanvas);
        
        // Default implementations for base interface
        ExecuteResult INodeExecutor.Execute(BaseNode baseNode)
        {
#if UNITY_EDITOR
            var result = Execute((TNode)baseNode);
            baseNode.ExecuteResult = result;
            return result;
#endif
            return Execute((TNode)baseNode);
        }
        
        ExecuteResult INodeExecutor.Cancel(BaseNode baseNode)
        {
#if UNITY_EDITOR
            var result = Cancel((TNode)baseNode);
            baseNode.CancelResult = result;
            return result;
#endif
            return Cancel((TNode)baseNode);
        }
        
        BaseNode INodeExecutor.GetNext(BaseNode baseNode) => GetNext((TNode)baseNode);
        
        void INodeExecutor.ResetExecutor(BaseNode baseNode)
        {
            try
            {
                ResetExecutor((TNode)baseNode);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error resetting executor from {baseNode.GetType().FullName} to {typeof(TNode).FullName}: {e}");
            }
        }
        
        void INodeExecutor.InitializeView(UICanvas uiCanvas)
        {
            if (uiCanvas is TCanvas canvas)
            {
                InitializeView(canvas);
            }
            else
            {
                Debug.LogError($"Canvas type mismatch: expected {typeof(TCanvas).Name}, got {uiCanvas?.GetType().Name}");
            }
        }
    }
    
    // For legacy executors that don't need view functionality
    public interface INodeExecute<in T> : INodeExecutor
        where T : BaseNode
    {
        BaseNode GetNext(T baseNode);
        ExecuteResult Execute(T baseNode);
        ExecuteResult Cancel(T baseNode);
        void ResetExecutor(T baseNode);

        ExecuteResult INodeExecutor.Execute(BaseNode baseNode)
        {
#if UNITY_EDITOR
            var result = Execute((T)baseNode);
            baseNode.ExecuteResult = result;
            return result;
#endif
            return Execute((T)baseNode);
        }
        
        ExecuteResult INodeExecutor.Cancel(BaseNode baseNode)
        {
#if UNITY_EDITOR
            var result = Cancel((T)baseNode);
            baseNode.CancelResult = result;
            return result;
#endif
            return Cancel((T)baseNode);
        }
        
        BaseNode INodeExecutor.GetNext(BaseNode baseNode) => GetNext((T)baseNode);
        
        void INodeExecutor.ResetExecutor(BaseNode baseNode)
        {
            try
            {
                ResetExecutor((T)baseNode);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error resetting executor from {baseNode.GetType().FullName} to {typeof(T).FullName}: {e}");
            }
        }
        
        // Default empty implementations for view methods
        void INodeExecutor.InitializeView(UICanvas uiCanvas) { }
        void INodeExecutor.ResetView() { }
    }
}