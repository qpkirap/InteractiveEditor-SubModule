﻿namespace Module.InteractiveEditor.Editor
{
    public class InspectorNodeVisitor : INodeVisitor
    {
        private readonly InspectorView inspectorView;
        private NodeView nodeViewCache;
        
        public InspectorNodeVisitor(InspectorView inspectorView)
        {
            this.inspectorView = inspectorView;
        }

        public void OnSelectNode(NodeView nodeView)
        {
            nodeViewCache = nodeView;
            
            nodeView.Visit(this);
        }

        public void BaseDialogueNodeView(BaseDialogueNodeView nodeView)
        {
            // Node view now handles its own updates through Odin Inspector
            // No need for custom editor injection
        }
    }
}