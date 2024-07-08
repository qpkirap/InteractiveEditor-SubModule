using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class ConditionsData : BaseData<IConditionComponent>
    {
        [field: SerializeField, SerializeReference, TabGroup("Conditions"), ListDrawerSettings(Expanded = true, ListElementLabelName = "GetTitle")] 
        public List<IConditionComponent> Conditions { get; private set; } = new();
                
#if UNITY_EDITOR
        public override List<IConditionComponent> DefaultComponents => new List<IConditionComponent>(0);
#endif
    }
}