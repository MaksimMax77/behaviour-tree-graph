using System.Linq;
using BehaviorTreeGraph.Runtime.Core.Nodes;

namespace BehaviorTreeGraph.Runtime.Core
{
    public sealed class BehaviorTreeRunner
    {
        private BehaviorTreeConfiguration _config;
        private BehaviorTreeNode.INodeState _rootState;

        public void Init(BehaviorTreeConfiguration config)
        {
            _config = config;
            
            foreach (var node in _config.Nodes)
            {
                node.GetNodeState();
            }
            var rootNode = FindRootNode();

            if (rootNode == null)
            {
                  throw new System.Exception("BehaviorTree has no root node");
            }
           
            _rootState = rootNode.GetNodeState();
            _rootState.OnEnter();
        }

        public void Update()
        {
            _rootState?.OnUpdate();
        }

        private BehaviorTreeNode FindRootNode()//todo нужно определять рутовую ноду иным способом 
        {
            foreach (var node in _config.Nodes)
            {
                var isChild = false;

                foreach (var other in _config.Nodes)
                {
                    if (other is CompositeNode composite &&
                        composite.Children.Contains(node))
                    {
                        isChild = true;
                        break;
                    }
                }

                if (!isChild)
                {
                    return node;
                }
            }

            return null;
        }
    }
}


