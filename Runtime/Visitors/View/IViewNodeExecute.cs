using Managers.Router;

namespace Module.InteractiveEditor.Runtime
{
    public interface IViewNodeExecute
    {
        void Inject(INodeExecute execute, UICanvas uiCanvas);
        
        void Reset();
    }
    
    public interface IViewNodeExecute<in TNodeExecute, in TUICanvas> : IViewNodeExecute
        where TNodeExecute : INodeExecute
        where TUICanvas : UICanvas
    {
        void IViewNodeExecute.Inject(INodeExecute execute, UICanvas uiCanvas)
        {
            Inject((TNodeExecute) execute, (TUICanvas) uiCanvas);
        }

        void Inject(TNodeExecute execute, TUICanvas uiCanvas);
    }
}