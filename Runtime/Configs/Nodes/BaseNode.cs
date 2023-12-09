using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Module.Utils.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseNode<T> : BaseNode
        where T : INodeExecute
    {
        public override Type GetExecutorType()
        {
            return typeof(T);
        }
    }
    
    public abstract class BaseNode : ScriptableEntity
    {
        [HideInInspector][SerializeField] private List<BaseNode> childrenNodes = new(); //input nodes

        #region Editor

        [HideInInspector][SerializeField] private Vector2 positionEditor;
        [SerializeField][TextArea] private string description;

        public const string PositionEditorKey = nameof(positionEditor);
        public const string ChildNodeKey = nameof(childrenNodes);
        public const string DescriptionKey = nameof(description);

        public ExecuteResult ExecuteResult { get; protected set; }
        public ExecuteResult CancelResult { get; protected set; }

        #endregion

        public IReadOnlyList<BaseNode> ChildrenNodes => childrenNodes;

        public abstract Type GetExecutorType();

        public ExecuteResult ExecuteTask(INodeExecute execute)
        {
            ExecuteResult = execute.Execute(this);
            
            return ExecuteResult;
        }

        public ExecuteResult CancelTask(INodeExecute execute)
        {
            CancelResult = execute.Cancel(this);
            
            return CancelResult;
        }

        public BaseNode GetNextNode(INodeExecute execute)
        {
            return execute.GetNext(this);
        }

        public override object Clone()
        {
            var clone = (BaseNode)base.Clone();
            
            clone.RemoveCloneSuffix();
            
            clone.childrenNodes = new();

            if (childrenNodes != null)
            {
                foreach (var children in childrenNodes)
                {
                    if (children == null) continue;

                    var childrenClone = (BaseNode)children.Clone();
                    
                    childrenClone.SetId(children.Id);
                    
                    clone.childrenNodes.Add(childrenClone);
                }
            }

            return clone;
        }
    }

    [Flags]
    public enum ExecuteResult
    {
        NoneState = 1,
        RunningState = 2,
        SuccessState = 4,
    }
}