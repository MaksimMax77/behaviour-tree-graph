using BehaviorTreeGraph.Runtime.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeGraph.Editor.NodeViews
{
    [NodeView(typeof(SequenceNode))]
    public class SequenceNodeView : NodeView
    {
        private Port _outputPort;
        public Port OutputPort => _outputPort;

        public SequenceNodeView(string title, Vector2 startPos, VisualElement inspector) : base(title, startPos, inspector)
        {
            CreateOutputPort();
        }

        private void CreateOutputPort()
        {
            _outputPort = InstantiatePort(
                Orientation.Horizontal,
                Direction.Output,
                Port.Capacity.Multi,
                typeof(bool)
            );
            _outputPort.portName = "Out";
            outputContainer.Add(_outputPort);
            RefreshExpandedState();
            RefreshPorts();
        }
    }
}