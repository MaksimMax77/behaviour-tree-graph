using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTreeGraph.Runtime.Core.Nodes
{
    public abstract class CompositeNode : BehaviorTreeNode
    {
        [SerializeField] private List<BehaviorTreeNode> _children = new List<BehaviorTreeNode>();

        public IReadOnlyList<BehaviorTreeNode> Children => _children;
        
        public void AddChild(BehaviorTreeNode child)
        {
            if (child != null && !_children.Contains(child))
            {
                _children.Add(child);
            }
        }

        public void RemoveChild(BehaviorTreeNode child)
        {
            if (child != null)
            {
                _children.Remove(child);
            }
        }

        protected abstract class CompositeNodeState<T> : NodeState<T>
            where T : CompositeNode
        {
            protected readonly INodeState[] _children;

            protected CompositeNodeState(T node) : base(node)
            {
                var childNodes = node.Children;
                _children = new INodeState[childNodes.Count];

                for (int i = 0; i < childNodes.Count; i++)
                {
                    _children[i] = childNodes[i].GetNodeState();
                }
            }
        }
    }
}