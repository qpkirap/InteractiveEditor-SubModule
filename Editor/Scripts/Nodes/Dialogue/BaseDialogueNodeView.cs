using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Base", typeof(BaseDialogueNode))]
    public class BaseDialogueNodeView : NodeView
    {
        private readonly ImageListVisualElement imagePreview;
        
        public BaseDialogueNodeView(BaseNode node) : base(node)
        {
            var imageElement = this.Q<VisualElement>(VisualElementKeys.ImagePreview);
            var listImages = node.GetFieldValue<List<AssetReference>>(BaseDialogueNode.ImagesKey);
            this.imagePreview = new ImageListVisualElement(imageElement, listImages);
        }

        public override Type InputPortType => typeof(bool);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Single;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Single;
        public override string GetClassTag => "dialogue";

        public override void AddChildNode(BaseNode node)
        {
            Undo.RecordObject(Node, "Add Child Node");
            
            Node.AddToList(BaseNode.ChildNodeKey, node);
            
            EditorUtility.SetDirty(Node);
        }

        public override void RemoveChildNode(BaseNode node)
        {
            Undo.RecordObject(Node, "Remove Child Node");
         
            Node.RemoveFromList(BaseNode.ChildNodeKey, node);
            
            EditorUtility.SetDirty(Node);
        }
    }
}