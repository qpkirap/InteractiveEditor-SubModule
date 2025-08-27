﻿﻿﻿using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [CreateAssetMenu(fileName = "ConditionsData", menuName = "Interactive Editor/Conditions Data")]
    public class ConditionsData : BaseData
    {
        [SerializeField, SerializeReference, TabGroup("Conditions"), ListDrawerSettings(Expanded = true, ListElementLabelName = "@GetTitle()")]
        private List<IConditionComponent> conditions = new();
        
        public List<IConditionComponent> Conditions => conditions;
        
#if UNITY_EDITOR
        public override List<IBaseComponent> DefaultComponents => new();
#endif
        
        // Helper method to get condition-specific components
        public T GetCondition<T>() where T : class, IConditionComponent
        {
            return Conditions?.Find(c => c is T) as T;
        }
        
        public bool HasCondition<T>() where T : class, IConditionComponent
        {
            return Conditions?.Exists(c => c is T) ?? false;
        }
    }
}