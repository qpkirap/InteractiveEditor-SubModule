using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public interface INodeExecute
    {
        ExecuteResult Execute(BaseNode baseNode);
        ExecuteResult Cancel(BaseNode baseNode);
        
        BaseNode GetNext(BaseNode baseNode);
        
        void ResetExecutor(BaseNode baseNode);
    }
    
    public interface INodeExecute<in T> : INodeExecute
        where T : BaseNode
    {
        BaseNode GetNext(T baseNode);
        ExecuteResult Execute(T baseNode);
        ExecuteResult Cancel(T baseNode);
        
        void ResetExecutor(T baseNode);

        ExecuteResult INodeExecute.Execute(BaseNode baseNode)
        {
            return Execute((T) baseNode);
        }
        
        ExecuteResult INodeExecute.Cancel(BaseNode baseNode)
        {
            return Cancel((T) baseNode);
        }
        
        BaseNode INodeExecute.GetNext(BaseNode baseNode)
        {
            return GetNext((T) baseNode);
        }
        
        void INodeExecute.ResetExecutor(BaseNode baseNode)
        {
            ResetExecutor((T) baseNode);
        }
    }
}