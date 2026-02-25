using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorTreeGraph.Runtime.Core.Nodes;
using UnityEditor;

namespace BehaviorTreeGraph.Editor.NodeViews
{
    public class NodeViewFactory
    {
        private readonly Dictionary<Type, Type> _nodeToViewMap;

        public NodeViewFactory()
        {
            _nodeToViewMap = new Dictionary<Type, Type>();
            BuildMap();
        }

        private void BuildMap()
        {
            var viewTypes = TypeCache
                .GetTypesDerivedFrom<NodeView>()
                .Where(t => !t.IsAbstract);

            foreach (var viewType in viewTypes)
            {
                var attr = viewType.GetCustomAttribute<NodeViewAttribute>();
                if (attr == null)
                    continue;

                _nodeToViewMap[attr.NodeType] = viewType;
            }
        }

        public NodeView Create(BehaviorTreeNode node)
        {
            var nodeInspectorRenderer = new NodeInspectorRenderer(node);
            var nodeInspector = nodeInspectorRenderer.CreateInspector();

            if (_nodeToViewMap.TryGetValue(node.GetType(), out var viewType))
            {
                return (NodeView)Activator.CreateInstance(
                    viewType, node.GetType().Name, node.Position, nodeInspector);
            }

            return new NodeView(node.GetType().Name, node.Position, nodeInspector);
        }
    }
}