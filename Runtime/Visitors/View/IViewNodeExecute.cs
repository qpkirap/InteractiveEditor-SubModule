using Managers.Router;

namespace Module.InteractiveEditor.Runtime
{
    public interface IViewNodeExecute
    {
        void Inject(INodeExecute execute, UICanvas uiCanvas);
        
        void Reset();
    }
    
    public interface IViewNodeExecute<in TINodeExecute, in TUICanvas> : IViewNodeExecute
        where TINodeExecute : INodeExecute
        where TUICanvas : UICanvas
    {
        void IViewNodeExecute.Inject(INodeExecute execute, UICanvas uiCanvas)
        {
            Inject((TINodeExecute) execute, (TUICanvas) uiCanvas);
        }

        void Inject(TINodeExecute execute, TUICanvas uiCanvas);
    }
}