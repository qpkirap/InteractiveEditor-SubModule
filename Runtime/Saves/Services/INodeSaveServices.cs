namespace Module.InteractiveEditor.Saves
{
    public interface INodeSaveServices
    {
        public SaveNodeItem GetSaveItem(string id);
        string GetIdLastNode(string idStory);
        void SetLastIdNode(string idStory, string idNode);
        void SetLastIdNode(string idNode);
    }
}