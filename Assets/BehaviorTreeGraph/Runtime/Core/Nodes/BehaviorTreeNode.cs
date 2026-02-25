using UnityEditor;
using UnityEngine;

namespace BehaviorTreeGraph.Runtime.Core.Nodes
{
    public abstract class BehaviorTreeNode : ScriptableObject
    {
        [SerializeField] private Vector2 _position;
        private INodeState _runtimeState;

        public Vector2 Position => _position;

        public void SetPosition(Vector2 position)
        {
            _position = position;
            EditorUtility.SetDirty(this);
        }

        public abstract INodeState GetNodeState();

        protected abstract class NodeState<T> : INodeState where T : BehaviorTreeNode
        {
            protected T _node;
            protected NodeStatus _status;

            public NodeStatus Status => _status;

            protected NodeState(T node)
            {
                _node = node;
                _status = NodeStatus.Running;
            }

            public virtual void OnEnter()
            {
            }

            public virtual void OnUpdate()
            {
            }

            public virtual void OnExit()
            {
            }
        }

        public interface INodeState
        {
            public NodeStatus Status { get; }
            public void OnEnter();
            public void OnUpdate();
            public void OnExit();
        }

        public enum NodeStatus
        {
            Running,
            Success,
            Failure
        }
    }
}