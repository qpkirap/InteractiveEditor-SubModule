﻿using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Module.InteractiveEditor.Runtime
{
    public class UICanvas<TNodeExecutor> : Managers.Router.UICanvas
        where TNodeExecutor : INodeExecutor, new()
    {
        protected TNodeExecutor nodeExecutor;
        protected readonly CompositeDisposable disp = new();

        public override async UniTask Init()
        {
            await base.Init();
            nodeExecutor ??= new TNodeExecutor();
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            nodeExecutor?.ResetView();

            var model = router.GetRoutArgData<INodeExecutor>(INodeExecutor.NodeExecutorKey);
            if (model != null)
            {
                nodeExecutor = (TNodeExecutor)model;
                nodeExecutor.InitializeView(this);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            disp.Clear();
        }
        
        protected virtual void OnDestroy()
        {
            if (disp is { IsDisposed: false }) 
                disp.Dispose();
        }
    }
}