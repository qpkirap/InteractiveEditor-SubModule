namespace Module.InteractiveEditor.Editor
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
            var currentEditor = inspectorView.GetCurrentEditor;
            
            if (currentEditor != null && currentEditor is BaseDialogueEditor dialogueEditor)
            {
                nodeView.InjectEditor(dialogueEditor);
            } 
        }
    }
}