using System;
using Module.InteractiveEditor.Configs;
using Newtonsoft.Json;

namespace Module.InteractiveEditor.Saves
{
    [Serializable]
    public class AnswerChoiceConditionSave : AnswerChoiceSave<AnswerChoiceConditionsDialogueNode>
    {
        public AnswerChoiceConditionSave(AnswerChoiceConditionsDialogueNode baseNode) : base(baseNode)
        {
        }
    }
    
    [Serializable]
    public class AnswerChoiceSave : AnswerChoiceSave<AnswerChoiceSaveDialogueNode>
    {
        public AnswerChoiceSave(AnswerChoiceSaveDialogueNode baseNode) : base(baseNode)
        {
        }
    }

    [Serializable]
    public abstract class AnswerChoiceSave<T> : SaveNodeItem<T>
        where T : BaseNode
    {
        protected AnswerChoiceSave(T baseNode) : base(baseNode)
        {
        }
        
        [JsonProperty("selected")] public bool IsSelected { get; private set; }

        public override void UpdateData()
        {
            IsSelected = true;
        }
    }
}