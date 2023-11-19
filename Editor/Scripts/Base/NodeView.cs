using System;
using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public abstract class NodeView : Node
    {
        private readonly Label description;
        private readonly ImageListVisualElement imagePreview;
        
        public BaseNode Node { get; protected set; }
        public Port InputPort { get; protected set; }
        public Port OutputPort { get; protected set; }
        
        public abstract Type InputPortType { get; }
        public abstract Type OutputPortType { get; }
        public abstract Port.Capacity InputPortCapacity { get; }
        public abstract Port.Capacity OutputPortCapacity { get; }
        public abstract string GetClassTag { get; }
        public IReadOnlyList<BaseNode> GetChildNodes() => Node.ChildrenNodes;
        public override string title => Node.Title;

        public Action<NodeView> OnSelectedNode;

        public NodeView(BaseNode node) : base(Paths.NodeViewUxml)
        {
            this.Node = node;
            this.viewDataKey = node.Id;

            var position = Node.GetFieldValue<Vector2>(BaseNode.PositionEditorKey);

            description = this.Q<Label>(VisualElementKeys.Description);
            description.bindingPath = BaseNode.DescriptionKey;
            description.Bind(new SerializedObject(node));

            style.left = position.x;
            style.top = position.y;
        }

        public virtual void Init()
        {
            CreateInputPortsLocal();
            
            CreateOutputPortsLocal();
            
            if (!string.IsNullOrEmpty(GetClassTag)) AddToClassList(GetClassTag);
        }
        
        public abstract void AddChildNode(BaseNode node);
        public abstract void RemoveChildNode(BaseNode node);

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            
            Undo.RecordObject(Node, "TreeView (Set Position)");
            
            Node.SetFieldValue(BaseNode.PositionEditorKey, new Vector2(newPos.xMin, newPos.yMin));
            
            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            
            OnSelectedNode?.Invoke(this);
        }

        protected virtual void CreateInputPortsLocal()
        {
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, InputPortCapacity, InputPortType);

            InputPort.portName = string.Empty;

            InputPort.style.flexDirection = FlexDirection.Column;

            inputContainer.Add(InputPort);
        }

        protected virtual void CreateOutputPortsLocal()
        {
            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, OutputPortCapacity, OutputPortType);

            OutputPort.portName = string.Empty;;
            
            OutputPort.style.flexDirection = FlexDirection.ColumnReverse;

            outputContainer.Add(OutputPort);
        }

        public void SortingChildren()
        {
            var children = Node.ChildrenNodes.ToList();
            
            children.Sort(SortByHorizontalPosition);
            
            Node.SetFieldValue(BaseNode.ChildNodeKey, children);
        }

        public void UpdateState()
        {
            RemoveFromClassList(ExecuteResult.NoneState.ToString());
            RemoveFromClassList(ExecuteResult.RunningState.ToString());
            RemoveFromClassList(ExecuteResult.SuccessState.ToString());
            RemoveFromClassList(ExecuteResult.BackState.ToString());
            RemoveFromClassList(ExecuteResult.ResetState.ToString());
            
            if (Application.isPlaying) AddToClassList(Node.ExecuteResult.ToString());
        }

        private int SortByHorizontalPosition(BaseNode left, BaseNode right)
        {
            var leftPosition = left.GetFieldValue<Vector2>(BaseNode.PositionEditorKey);
            var rightPosition = right.GetFieldValue<Vector2>(BaseNode.PositionEditorKey);
            
            return leftPosition.x < rightPosition.x ? 1 : -1;
        }
    }
}