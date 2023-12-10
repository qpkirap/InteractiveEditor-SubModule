using Managers.Router.Config;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public interface INodeExecute
    {
        public static RoutArgKey NodeExecutorKey = new("NodeExecutorKey");
        
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
#if UNITY_EDITOR
            var result = Execute((T) baseNode);
            
            baseNode.ExecuteResult = result;
            
            return result;
#endif
            
            return Execute((T) baseNode);
        }
        
        ExecuteResult INodeExecute.Cancel(BaseNode baseNode)
        {
#if UNITY_EDITOR
            var result = Cancel((T) baseNode);
            
            baseNode.CancelResult = result;
            
            return result;
#endif
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