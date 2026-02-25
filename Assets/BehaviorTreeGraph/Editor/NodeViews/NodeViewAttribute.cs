using System;

namespace BehaviorTreeGraph.Editor.NodeViews
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NodeViewAttribute : Attribute
    {
        public Type NodeType { get; }

        public NodeViewAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}