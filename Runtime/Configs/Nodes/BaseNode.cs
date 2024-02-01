using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;
using Module.Utils.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseNode<TNodeExecutor, TSaveNodeItem> : BaseNode
        where TNodeExecutor : INodeExecute
        where TSaveNodeItem : SaveNodeItem
    {
        public override Type GetSaveItemType()
        {
            return typeof(TSaveNodeItem);
        }
        
        public override Type GetExecutorType()
        {
            return typeof(TNodeExecutor);
        }
    }
    
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

        public ExecuteResult ExecuteResult { get; set; }
        public ExecuteResult CancelResult { get; set; }

        #endregion

        public IReadOnlyList<BaseNode> ChildrenNodes => childrenNodes;

        public abstract IReadOnlyCollection<IAddressableAsset> GetAssets();
        public abstract Type GetExecutorType();

        public virtual Type GetSaveItemType()
        {
            return default;
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