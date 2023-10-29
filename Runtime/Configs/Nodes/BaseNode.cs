using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseNode : ScriptableEntity
    {
        [HideInInspector][SerializeField] private List<BaseNode> childrenNodes = new(); //input nodes

        public ExecuteResult ExecuteResult { get; protected set; } = ExecuteResult.NoneState;
        public ExecuteResult CancelResult { get; protected set; } = ExecuteResult.NoneState;
        public abstract NodeType NodeType { get; }

        #region Editor

        [HideInInspector][SerializeField] private Vector2 positionEditor;

        public const string PositionEditorKey = nameof(positionEditor);
        public const string ChildNodeKey = nameof(childrenNodes);

        #endregion

        public IReadOnlyList<BaseNode> ChildrenNodes => childrenNodes;

        protected abstract ExecuteResult ExecuteTask();
        protected abstract ExecuteResult CancelTask();

        public ExecuteResult Execute()
        {
            ExecuteResult = ExecuteTask();

            return ExecuteResult;
        }

        public ExecuteResult Cancel()
        {
            ExecuteResult = ExecuteResult.NoneState;
            CancelResult = ExecuteResult.NoneState;
            
            CancelResult = CancelTask();

            return CancelResult;
        }

        public override object Clone()
        {
            var item = (BaseNode)base.Clone();

            item.childrenNodes = new();

            if (childrenNodes != null)
            {
                foreach (var children in childrenNodes)
                {
                    if (children == null) continue;
                    
                    item.childrenNodes.Add((BaseNode)children.Clone());
                }
            }

            return item;
        }
    }
    
    public enum NodeType
    {
        Dialogue
    }

    [Flags]
    public enum ExecuteResult
    {
        NoneState = 1,
        RunningState = 2,
        SuccessState = 4,
        BackState = 8,
        ResetState = 16
    }
}