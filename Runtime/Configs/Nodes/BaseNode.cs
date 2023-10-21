using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseNode : ScriptableEntity
    {
        [SerializeField] private List<BaseNode> childrenNodes = new(); //input nodes

        public ExecuteResult ExecuteResult { get; protected set; }
        public ExecuteResult CancelResult { get; protected set; }
        public abstract NodeType NodeType { get; }

        #region Editor

        [SerializeField] private Vector2 positionEditor;

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
            CancelResult = CancelTask();

            return CancelResult;
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
        SkipState = 8,
        ResetState = 16
    }
}