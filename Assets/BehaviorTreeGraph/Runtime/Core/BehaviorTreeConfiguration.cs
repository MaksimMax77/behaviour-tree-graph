using System.Collections.Generic;
using BehaviorTreeGraph.Runtime.Core.Nodes;
using UnityEngine;

namespace BehaviorTreeGraph.Runtime.Core
{
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tree Data")]
    public class BehaviorTreeConfiguration : ScriptableObject
    {
        public string TreeName = "New Behavior Tree";
        public List<BehaviorTreeNode> Nodes = new List<BehaviorTreeNode>();
    }
}
