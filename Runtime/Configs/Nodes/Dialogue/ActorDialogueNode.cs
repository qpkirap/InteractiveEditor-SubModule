using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class ActorDialogueNode : BaseDialogueNode
    {
        [HideInInspector][SerializeField] private Actor actor;
        
        public Actor Actor => actor;

        public const string ActorKey = nameof(actor);
    }
}