using System.Collections.Generic;
using BehaviorTreeGraph.Editor.NodeViews;
using BehaviorTreeGraph.Runtime.Core.Nodes;
using Unity.VisualScripting;

namespace BehaviorTreeGraph.Editor
{
    public class CreatedNodesManager
    {
        private readonly Dictionary<NodeView, BehaviorTreeNode> _nodes = new();

        public void AddAndSubscribe(NodeView nodeView, BehaviorTreeNode node)
        {
            nodeView.PositionChanged += node.SetPosition;
            _nodes.Add(nodeView, node);
        }

        public void AddChildToCompositeNode(NodeView parent, NodeView child)
        {
            if (!_nodes.TryGetValue(parent, out var parentNode)
                || !_nodes.TryGetValue(child, out var childNode))
            {
                return;
            }

            if (parentNode is not CompositeNode compositeNode)
            {
                return;
            }

            compositeNode.AddChild(childNode);
            UpdateChildIndices(compositeNode);
        }

        public void RemoveChildFromCompositeNode(NodeView parent, NodeView child)
        {
            if (!_nodes.TryGetValue(parent, out var parentNode)
                || !_nodes.TryGetValue(child, out var childNode))
            {
                return;
            }

            if (parentNode is not CompositeNode compositeNode)
            {
                return;
            }

            compositeNode.RemoveChild(childNode);
            UpdateChildIndices(compositeNode);
        }
      
        public void RestoreEdges(System.Action<NodeView, NodeView> connect)
        {
            foreach (var kvp in _nodes)
            {
                var parentView = kvp.Key;
                var parentNode = kvp.Value;

                if (parentNode is not CompositeNode composite)
                    continue;

                foreach (var child in composite.Children)
                {
                    var childView = GetView(child);
                    if (childView == null)
                        continue;

                    connect(parentView, childView);
                }
            }
        }

        public void ShowOrderViewForChild()
        {
            foreach (var kvp in _nodes)
            {
                var parentNode = kvp.Value;

                if (parentNode is not CompositeNode composite)
                    continue;
                
                foreach (var child in composite.Children)
                {
                    var childView = GetView(child);
                    if (childView == null)
                        continue;
                    childView.ChildOrderView.Show();
                }
                UpdateChildIndices(composite);
            }
        }
        
        private void UpdateChildIndices(CompositeNode node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                var childView = GetView(child);
                childView.ChildOrderView.SetIndex(i);
            }
        }
        
        private NodeView GetView(BehaviorTreeNode node)
        {
            foreach (var kvp in _nodes)
            {
                if (kvp.Value == node)
                    return kvp.Key;
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var node in _nodes)
            {
                node.Key.PositionChanged -= node.Value.SetPosition;
            }

            _nodes.Clear();
        }
    }
}
