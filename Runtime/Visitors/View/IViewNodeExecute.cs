﻿using Managers.Router;

namespace Module.InteractiveEditor.Runtime
{
    // Legacy interface - use INodeExecutor instead
    [System.Obsolete("Use INodeExecutor interface instead. This interface will be removed in future versions.")]
    public interface IViewNodeExecute
    {
        void Inject(INodeExecutor execute, UICanvas uiCanvas);
        void Reset();
    }
    
    [System.Obsolete("Use INodeExecutor<TNode, TCanvas> interface instead. This interface will be removed in future versions.")]
    public interface IViewNodeExecute<in TNodeExecutor, in TUICanvas> : IViewNodeExecute
        where TNodeExecutor : INodeExecutor
        where TUICanvas : UICanvas
    {
        void IViewNodeExecute.Inject(INodeExecutor execute, UICanvas uiCanvas)
        {
            Inject((TNodeExecutor)execute, (TUICanvas)uiCanvas);
        }

        void Inject(TNodeExecutor execute, TUICanvas uiCanvas);
    }
}