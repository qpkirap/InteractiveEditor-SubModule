using Module.Utils.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public abstract class Component : ScriptableEntity
    {
        public override object Clone()
        {
            var newComponent = base.Clone() as Component;

            newComponent.RemoveCloneSuffix();

            return newComponent;
        }
    }
}