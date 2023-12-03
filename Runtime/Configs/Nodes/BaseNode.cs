using System;
using System.Collections.Generic;
using Module.Utils.Configs;
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
        [SerializeField][TextArea] private string description;

        public const string PositionEditorKey = nameof(positionEditor);
        public const string ChildNodeKey = nameof(childrenNodes);
        public const string DescriptionKey = nameof(description);

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
    
    public enum NodeType
    {
        Dialogue,
        Action
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