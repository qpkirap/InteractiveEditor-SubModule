using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceConditionDialogueNode : AnswerChoiceDialogueNode
    {
        [field: SerializeField] public IConditionComponent Conditions { get; private set; }

        private const string ConditionsKey = nameof(Conditions);

        public override object Clone()
        {
            var item = base.Clone();
            
            item.SetFieldValue(ConditionsKey, Conditions);
            
            return item;
        }
    }
}