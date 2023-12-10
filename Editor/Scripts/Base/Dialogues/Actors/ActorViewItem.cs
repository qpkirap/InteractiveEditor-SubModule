using Module.InteractiveEditor.Configs;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ActorViewItem : Button
    {
        public Actor Actor { get; private set; }

        public ActorViewItem()
        {
            AddToClassList("actor-item");
        }

        public void InjectData(Actor actor)
        {
            this.Actor = actor;

            text = actor.Title;
        }
    }
}