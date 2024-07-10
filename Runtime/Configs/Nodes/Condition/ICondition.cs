using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public interface ICondition
    {
        public bool IsTrue(BaseNode baseNode);
    }
}