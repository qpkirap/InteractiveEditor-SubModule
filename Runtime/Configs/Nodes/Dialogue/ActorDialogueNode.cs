using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class ActorDialogueNode : BaseDialogueNode
    {
        [SerializeField] private Actor actor;
        
        public Actor Actor => actor;
    }
}