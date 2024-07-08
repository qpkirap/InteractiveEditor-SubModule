using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public abstract class ConditionComponent<TCondition> : BaseComponent, IConditionComponent
        where TCondition : ICondition
    {
        public abstract TCondition GetCondition(params object[] args);
    }
    
    public interface IConditionComponent : IBaseComponent
    {
    }
}