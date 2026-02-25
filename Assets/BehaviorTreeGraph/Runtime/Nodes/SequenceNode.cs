using BehaviorTreeGraph.Runtime.Core;
using BehaviorTreeGraph.Runtime.Core.Nodes;

namespace BehaviorTreeGraph.Runtime.Nodes
{
    [BehaviorNodeMenu("Composite/SequenceNode")]
    public class SequenceNode : CompositeNode
    {
        public override INodeState GetNodeState()
        {
            return new SequenceNodeState(this);
        }

        protected class SequenceNodeState : CompositeNodeState<SequenceNode>
        {
            private int _currentIndex;

            public SequenceNodeState(SequenceNode node) : base(node)
            {
                
            }
            
            public override void OnEnter()
            {
                _currentIndex = 0;
                _status = NodeStatus.Running;

                if (_children.Length > 0)
                {
                     _children[0].OnEnter();
                }
            }

            public override void OnUpdate()
            {
                if (_currentIndex >= _children.Length)
                {
                    _status = NodeStatus.Success;
                    return;
                }

                var current = _children[_currentIndex];
                current.OnUpdate();

                
                var childStatus = current.Status;

                switch (childStatus)
                {
                    case NodeStatus.Running:
                        _status = NodeStatus.Running;
                        break;
                    case NodeStatus.Failure:
                        _status = NodeStatus.Failure;
                        break;
                    case NodeStatus.Success:
                    {
                        current.OnExit();
                        _currentIndex++;
                        if (_currentIndex < _children.Length)
                            _children[_currentIndex].OnEnter();
                        else
                            _status = NodeStatus.Success;
                        break;
                    }
                }
            }

            public override void OnExit()
            {
                if (_currentIndex < _children.Length)
                {
                    _children[_currentIndex].OnExit();
                }
            }
        }
    }
}