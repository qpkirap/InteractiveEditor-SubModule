using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;
using Newtonsoft.Json;
using UnityEngine;

namespace Module.InteractiveEditor.Saves
{
    [Serializable]
    public class DialogueSelectChoiceSave : SaveNodeItem<DialogueSelectChoiceSaveExecutor>
    {
        [JsonProperty("selected")]private string selectedId;

        public DialogueSelectChoiceSave(DialogueSelectChoiceSaveExecutor executor, BaseNode baseNode) : base(executor, baseNode)
        {
        }

        public override void UpdateData()
        {
            if (executor == null 
                || baseNode == null 
                || executor.SelectedIndex < 0 
                || baseNode.ChildrenNodes == null 
                || baseNode.ChildrenNodes.Count == 0) 
                return;

            var choice = baseNode.ChildrenNodes[Mathf.Clamp(executor.SelectedIndex, 0, baseNode.ChildrenNodes.Count - 1)];
            
            selectedId = choice.Id;
        }
    }
}