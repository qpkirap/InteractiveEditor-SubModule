using System;
using Cysharp.Threading.Tasks;

namespace Module.InteractiveEditor.Runtime
{
    public class UICanvas<TViewNode> : Managers.Router.UICanvas
        where TViewNode : IViewNodeExecute
    {
        protected TViewNode viewModel;
        
        public override async UniTask Init()
        {
            await base.Init();

            viewModel ??= Activator.CreateInstance<TViewNode>();

            viewModel.Reset();

            var model = router.GetRoutArgData<INodeExecute>(INodeExecute.NodeExecutorKey);
            
            viewModel.Inject(model, this);
        }
    }
}