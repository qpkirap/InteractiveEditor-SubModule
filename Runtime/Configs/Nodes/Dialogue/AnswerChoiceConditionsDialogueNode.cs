using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceConditionsDialogueNode : AnswerChoiceDialogueNode
    {
        [field: SerializeField, SerializeReference, ListDrawerSettings(Expanded = true),  OnValueChanged(nameof(OnGenerateId))] public List<IConditionComponent> Conditions = new();

        private const string ConditionsKey = nameof(Conditions);

        public override object Clone()
        {
            var item = base.Clone();
            
            item.SetFieldValue(ConditionsKey, Conditions);
            
            return item;
        }
        
        private void OnGenerateId()
        {
            Conditions.ForEach(item => item.GenerateId());
        }
    }
}