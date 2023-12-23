namespace Module.InteractiveEditor.Editor
{
    public interface INodeVisitor
    {
        void OnSelectNode(NodeView nodeView);
        void BaseDialogueNodeView(BaseDialogueNodeView nodeView);
    }
}