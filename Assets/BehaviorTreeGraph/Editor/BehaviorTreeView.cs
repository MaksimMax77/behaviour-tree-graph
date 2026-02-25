using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorTreeGraph.Editor.NodeViews;
using BehaviorTreeGraph.Runtime.Core;
using BehaviorTreeGraph.Runtime.Core.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeGraph.Editor
{
    public class BehaviorTreeView : GraphView
    {
        private BehaviorTreeConfiguration _currentTree;
        private CreatedNodesManager _createdNodesManager;
        private NodeViewFactory _nodeViewFactory;

        public BehaviorTreeView()
        {
            _nodeViewFactory = new NodeViewFactory();
            _createdNodesManager = new CreatedNodesManager();
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            AddGridBackground();
            AddManipulators();
            AddMiniMap();
            AddStyles();
            graphViewChanged += OnGraphViewChanged;
        }
        
        private void AddGridBackground()
        {
            var grid = new GridBackground();
            grid.SendToBack();
            grid.StretchToParentSize();
            Insert(0, grid);
        }
        
        private void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }
        
        private void AddMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };

            RegisterCallback<GeometryChangedEvent>(_ =>
            {
                float width = 200;
                float height = 140;
                miniMap.SetPosition(new Rect(
                    contentContainer.layout.width - width - 10,
                    contentContainer.layout.height - height - 10,
                    width,
                    height
                ));
            });

            Add(miniMap);
        }
        
        private void AddStyles()
        {
            var styleSheet = (StyleSheet)EditorGUIUtility
                .Load("Assets/BehaviorTreeGraph/Editor/Styles/BehaviorTreeGraphStyle.uss");
            styleSheets.Add(styleSheet);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    if (edge.output.node is SequenceNodeView sequence)
                    {
                        var child = edge.input.node as NodeView;
                        child?.ChildOrderView.Show();
                        _createdNodesManager.AddChildToCompositeNode(sequence, child);
                    }
                }
            }
            
            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        if (edge.output.node is SequenceNodeView sequence)
                        {
                            var child = edge.input.node as NodeView;
                            child?.ChildOrderView.Hide();
                            _createdNodesManager.RemoveChildFromCompositeNode(sequence, child);
                        }
                    }
                }
            }

            return change;
        }
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.node != startPort.node &&
                endPort.direction != startPort.direction &&
                endPort.portType == startPort.portType
            ).ToList();
        }

        public void SetTree(BehaviorTreeConfiguration tree)
        {
            _currentTree = tree;
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var nodeTypes = TypeCache
                .GetTypesDerivedFrom<BehaviorTreeNode>()
                .Where(t => !t.IsAbstract);

            var spawnPos = evt.localMousePosition;

            foreach (var type in nodeTypes)
            {
                var attr = type.GetCustomAttribute<BehaviorNodeMenuAttribute>();
                if (attr == null)
                    continue;

                evt.menu.AppendAction(
                    $"Add/{attr.Path}",
                    _ => CreateNode(type, spawnPos)
                );
            }
        }

        private void CreateNode(Type nodeType, Vector2 position)
        {
            if (_currentTree == null)
                return;

            if (ScriptableObject.CreateInstance(nodeType) is not BehaviorTreeNode nodeData)
                return;
            
            nodeData.SetPosition(position);
            var nodeView = _nodeViewFactory.Create(nodeData);
           _createdNodesManager.AddAndSubscribe(nodeView, nodeData);
            AddElement(nodeView);
            
            AssetDatabase.AddObjectToAsset(nodeData, _currentTree);
            _currentTree.Nodes.Add(nodeData);
            EditorUtility.SetDirty(_currentTree);
            AssetDatabase.SaveAssets();
        }

        public void LoadNodesFromConfiguration()
        {
            ClearGraph();

            if (_currentTree == null)
                return;

            foreach (var nodeData in _currentTree.Nodes)
            {
                var nodeView = _nodeViewFactory.Create(nodeData);
                _createdNodesManager.AddAndSubscribe(nodeView, nodeData);
                AddElement(nodeView);
            }
            
            _createdNodesManager.RestoreEdges((parentNodeView, childNodeView) =>
            {
                if (parentNodeView is not SequenceNodeView compositeNodeView)
                {
                    return;
                }
                var edge = compositeNodeView.OutputPort.ConnectTo(childNodeView.InputPort);
                AddElement(edge);
            });

            _createdNodesManager.ShowOrderViewForChild();
        }

        private void ClearGraph()
        {
            _createdNodesManager.Dispose();
            
            foreach (var view in nodes.ToArray())
            {
                RemoveElement(view);
            }
        }
    }
}