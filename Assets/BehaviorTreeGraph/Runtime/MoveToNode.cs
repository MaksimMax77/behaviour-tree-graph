using BehaviorTreeGraph.Runtime.Core;
using BehaviorTreeGraph.Runtime.Core.Nodes;

namespace BehaviorTreeGraph.Runtime
{
    [BehaviorNodeMenu("Action/MoveTo")]
    public class MoveToNode : BehaviorTreeNode
    {
        public float speed = 5f;

        public override INodeState GetNodeState()
        {
            return new MoveToNodeState(this);
        }
        
        private class MoveToNodeState: NodeState<MoveToNode>
        {
            public MoveToNodeState(MoveToNode node) : base(node)
            {
            }
        }
    }
}
