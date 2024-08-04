using System;
using Module.InteractiveEditor.Runtime;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class CheckAnswerConditionComponent : ConditionComponent<CheckAnswerCondition>
    {
        [field: SerializeField] public string IdNode { get; private set; }
        [field: SerializeField] public bool IsInverse { get; private set; }
        protected override CheckAnswerCondition GetCondition(params object[] args)
        {
            return new CheckAnswerCondition(this);
        }
    }
}