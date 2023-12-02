using System.Threading;
using Cysharp.Threading.Tasks;

namespace Module.InteractiveEditor.Runtime
{
    public abstract class ActionTaskComponent<TAction> : ActionTaskComponent
        where TAction : IActionTask
    {
        private TAction actionCache;
        
        public virtual TAction GetAction()
        {
            return default;
        }
        
        public override async UniTask Execute(CancellationToken token)
        {
            actionCache ??= GetAction();
            
            if (actionCache == null) return;

            await actionCache.Execute(token);
        }
        
        public override async UniTask Undo(CancellationToken token)
        {
            actionCache ??= GetAction();
            
            if (actionCache == null) return;
            
            await actionCache.Undo(token);
        }
    }
    
    public abstract class ActionTaskComponent : Component
    {
        public abstract UniTask Execute(CancellationToken token);
        public abstract UniTask Undo(CancellationToken token);
    }
}