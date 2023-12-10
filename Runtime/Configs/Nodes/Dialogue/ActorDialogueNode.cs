using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class ActorDialogueNode : BaseDialogueNode<BaseDialogueExecutor>
    {
        [HideInInspector][SerializeField] private Actor actor;
        
        public Actor Actor => actor;

        public const string ActorKey = nameof(actor);
        
        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(ActorKey, actor);
            
            return item;
        }
    }
}