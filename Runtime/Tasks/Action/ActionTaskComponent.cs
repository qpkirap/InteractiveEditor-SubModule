using System.Threading;
using Cysharp.Threading.Tasks;

namespace Module.InteractiveEditor.Runtime
{
    public abstract class ActionTaskComponent<TAction> : ActionTaskComponent
        where TAction : IActionTask
    {
    }
    
    public abstract class ActionTaskComponent : Component
    {
        public abstract IActionTask GetAction();
    }
}