using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeGraph.Editor.NodeViews
{
    public class NodeView : Node
    {
        public event Action<Vector2> PositionChanged;
        private Port _inputPort;
        private CompositeChildOrderView _childOrderView;
        public Port InputPort => _inputPort;
        public CompositeChildOrderView ChildOrderView => _childOrderView;

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            PositionChanged?.Invoke(newPos.position);
        }

        public NodeView(string title,
            Vector2 startPos, VisualElement inspector)
        {
           this.title = title;

            style.left = startPos.x;
            style.top = startPos.y;
            
            _childOrderView = new CompositeChildOrderView(extensionContainer);
            _childOrderView.MoveRequested += OnMoveRequested; //todo сделать диспоус для нод
            
            extensionContainer.Add(inspector);
            RefreshExpandedState();
            CreateInputPort();
        }

        private void OnMoveRequested(bool up)
        {
            
        }
        
        private void CreateInputPort()
        {
            _inputPort = InstantiatePort(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Single,
                typeof(bool)
            );

            _inputPort.portName = "In";
            inputContainer.Add(_inputPort);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}

