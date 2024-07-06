using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Module.InteractiveEditor.Runtime
{
    public class UICanvas<TIViewNodeExecutor> : Managers.Router.UICanvas
        where TIViewNodeExecutor : IViewNodeExecute
    {
        protected TIViewNodeExecutor viewModel;
        
        protected readonly CompositeDisposable disp = new();
        
        public override async UniTask Init()
        {
            await base.Init();

            viewModel ??= Activator.CreateInstance<TIViewNodeExecutor>();
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            viewModel?.Reset();

            var model = router.Value.GetRoutArgData<INodeExecute>(INodeExecute.NodeExecutorKey);
            
            viewModel?.Inject(model, this);
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            disp.Clear();
        }
        
        protected virtual void OnDestroy()
        {
            if (disp is { IsDisposed: false }) disp.Dispose();
        }
    }
}