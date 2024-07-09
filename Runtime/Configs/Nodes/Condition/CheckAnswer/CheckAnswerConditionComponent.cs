using System;
using Module.InteractiveEditor.Runtime;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class CheckAnswerConditionComponent : ConditionComponent<CheckAnswerCondition>
    {
        [SerializeField] private string test;
        protected override CheckAnswerCondition GetCondition(params object[] args)
        {
            return new CheckAnswerCondition();
        }
    }
}