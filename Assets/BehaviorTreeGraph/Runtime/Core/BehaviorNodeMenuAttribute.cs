using System;

namespace BehaviorTreeGraph.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BehaviorNodeMenuAttribute : Attribute
    {
        public string Path { get; }

        public BehaviorNodeMenuAttribute(string path)
        {
            Path = path;
        }
    }
}
