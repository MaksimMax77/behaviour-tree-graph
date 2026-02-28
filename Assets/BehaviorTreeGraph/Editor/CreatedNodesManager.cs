using System.Collections.Generic;
using BehaviorTreeGraph.Editor.NodeViews;
using BehaviorTreeGraph.Runtime.Core;
using BehaviorTreeGraph.Runtime.Core.Nodes;
using UnityEditor;

namespace BehaviorTreeGraph.Editor
{
    public class CreatedNodesManager
    {
        private BehaviorTreeConfiguration _currentTree;
        private readonly Dictionary<NodeView, BehaviorTreeNode> _nodes = new();

        public void SetCurrentTree(BehaviorTreeConfiguration currentTree)
        {
            _currentTree = currentTree;
        }

        public void AddAndSubscribe(NodeView nodeView, BehaviorTreeNode node)
        {
            nodeView.PositionChanged += node.SetPosition;
            nodeView.MoveIndexRequested += OnMoveIndexRequested;
            _nodes.Add(nodeView, node);

            if (AssetDatabase.Contains(node))
            {
                return;
            }
            AssetDatabase.AddObjectToAsset(node, _currentTree);
            _currentTree.Nodes.Add(node);
            SaveTreeAsset();
        }

        public void RemoveAndDispose(NodeView nodeView)
        {
            if (!_nodes.TryGetValue(nodeView, out var node))
            {
                return;
            }
            nodeView.PositionChanged -= node.SetPosition;
            nodeView.MoveIndexRequested -= OnMoveIndexRequested;
            nodeView.Dispose();
            _nodes.Remove(nodeView);
            
            AssetDatabase.RemoveObjectFromAsset(node);
            _currentTree.Nodes.Remove(node);
            SaveTreeAsset();
        }

        private void SaveTreeAsset()
        {
            EditorUtility.SetDirty(_currentTree);
            AssetDatabase.SaveAssets();
        }

        public void Dispose()
        {
            foreach (var node in _nodes)
            {
                var nodeView = node.Key;
                nodeView.PositionChanged -= node.Value.SetPosition;
                nodeView.MoveIndexRequested -= OnMoveIndexRequested;
                nodeView.Dispose();
            }

            _nodes.Clear();
        }

        private void OnMoveIndexRequested(NodeView nodeView, bool up)
        {
            if (!_nodes.TryGetValue(nodeView, out var currentNode))
            {
                return;
            }

            if (!TryGetParent(currentNode, out var parentNode))
            {
                return;
            }
            
            var children = parentNode.Children;

            var oldIndex = children.IndexOf(currentNode);
            var newIndex = up ? oldIndex - 1 : oldIndex + 1;

            if (newIndex < 0 || newIndex >= children.Count)
                return;
            children.RemoveAt(oldIndex);
            children.Insert(newIndex, currentNode);

            UpdateChildIndices(parentNode);
        }

        private bool TryGetParent(BehaviorTreeNode currentNode, out CompositeNode parent)
        {
            parent = null;

            foreach (var kvp in _nodes)
            {
                var node = kvp.Value;

                if (node is not CompositeNode compositeNode)
                {
                    continue;
                }

                if (!compositeNode.Children.Contains(currentNode))
                {
                    continue;
                }
                
                parent = compositeNode;
                return true;
            }

            return false;
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
    }
}
