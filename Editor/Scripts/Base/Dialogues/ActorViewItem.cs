using Module.InteractiveEditor.Configs;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ActorViewItem : Button
    {
        public Actor Actor { get; private set; }

        public override string text => Actor != null ? Actor.name : "";

        public ActorViewItem()
        {
            AddToClassList("actor-item");
        }

        public void InjectData(Actor actor)
        {
            this.Actor = actor;
        }
    }
}