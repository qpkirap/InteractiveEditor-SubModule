using System;
using Module.InteractiveEditor.Configs;
using Newtonsoft.Json;

namespace Module.InteractiveEditor.Saves
{
    [Serializable]
    public class AnswerChoiceSave : SaveNodeItem<AnswerChoiceSaveDialogueNode>
    {
        [JsonProperty("selected")] public bool IsSelected { get; private set; }

        public AnswerChoiceSave(AnswerChoiceSaveDialogueNode baseNode) : base(baseNode)
        {
        }

        public override void UpdateData()
        {
            IsSelected = true;
        }
    }
}