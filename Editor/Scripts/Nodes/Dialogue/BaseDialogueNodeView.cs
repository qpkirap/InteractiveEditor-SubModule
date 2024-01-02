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
        private readonly ImageListVisualController imagePreview;
        private BaseDialogueEditor dialogueEditor;

        public BaseDialogueNodeView(BaseNode node) : base(node)
        {
            var imageElement = this.Q<VisualElement>(VisualElementKeys.ImagePreview);
            this.imagePreview = new ImageListVisualController(imageElement);
            
            UpdateImages();
        }

        public override Type InputPortType => typeof(bool);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Single;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Single;
        public override string GetClassTag => "dialogue";

        public void InjectEditor(BaseDialogueEditor dialogueEditor)
        {
            this.dialogueEditor = dialogueEditor;

            dialogueEditor.OnUpdate = null;
            dialogueEditor.OnUpdate += UpdateImages;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            
            UpdateImages();
        }

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

        public override void Visit(INodeVisitor visitor)
        {
            visitor.BaseDialogueNodeView(this);
        }
        
        private void UpdateImages()
        {
            var listImages = Node.GetFieldValue<List<ImageData>>(BaseDialogueNode.ImagesDataKey);
            
            imagePreview.Init(listImages);
        }
    }
}