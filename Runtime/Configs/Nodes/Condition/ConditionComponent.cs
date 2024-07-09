using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public abstract class ConditionComponent<TCondition> : BaseComponent, IConditionComponent
        where TCondition : ICondition
    {
        protected abstract TCondition GetCondition(params object[] args);

        ICondition IConditionComponent.GetCondition(params object[] args)
        {
            return GetCondition(args);
        }
    }
    
    public interface IConditionComponent : IBaseComponent
    {
        ICondition GetCondition(params object[] args);
    }
}