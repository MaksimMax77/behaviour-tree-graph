# Behaviour Tree Graph

Behaviour Tree Graph is a visual editor for creating and managing Behavior Trees in Unity. It’s built with GraphView and UI Toolkit, allowing you to design AI behavior visually and execute it at runtime.

---

## Features

- Visual editor inside Unity for Behavior Trees
- Node types: Composite, Action, Decorator
- Drag-and-drop node placement with automatic position saving
- Connect nodes with ports and smooth cubic edges
- Node inspector automatically renders serialized fields
- Reorder child nodes in Composite/Sequence nodes using Up/Down buttons
- Runtime execution via `BehaviorTreeRunner`

---

## Installation

1. Clone this repository into your Unity project under:

```
Assets/BehaviourTreeGraph
```

2. Open the editor window in Unity via:

```
Window → AI → Behavior Tree Editor
```

---

## Quick Start

### Creating a New Tree

1. Open **AI → Behavior Tree Editor**  
2. Click **Create New Behavior Tree**  
3. Drag nodes onto the graph  
4. Connect them into a sequence  
5. Save the tree and use it in your game  

### Using the Tree in Code

To run the behavior tree at runtime:

```csharp
var runner = new BehaviorTreeRunner();
runner.Init(treeConfig); // treeConfig is your BehaviorTreeConfiguration asset
runner.Update();         // call every frame to tick the tree
```

---

## Project Structure

```
Assets/BehaviourTreeGraph
├─ Runtime
│  ├─ Core
│  │  ├─ Nodes/
│  │  └─ BehaviorTreeConfiguration.cs
│  └─ Nodes/
├─ Editor
│  ├─ BehaviorTreeEditorWindow.cs
│  ├─ BehaviorTreeView.cs
│  ├─ NodeViews/
│  └─ ...
├─ Packages
├─ ProjectSettings
```

---

## Extending the Editor

To create a new node type, use the `BehaviorNodeMenuAttribute`:

```csharp
[BehaviorNodeMenu("Action/MyCustomAction")]
public class MyCustomActionNode : BehaviorTreeNode
{
    // implement node behavior here
}
```

This automatically adds your node to the editor’s context menu.

---

## How It Works

- **BehaviorTreeConfiguration** — stores the tree data as a ScriptableObject  
- **NodeViewFactory** — creates visual node views  
- **CreatedNodesManager** — maps `NodeView` ↔ `BehaviorTreeNode`  
- **GraphViewChange** — tracks connections and rebuilds edges  
- **BehaviorTreeRunner** — executes the behavior tree at runtime  

---

## Composite Node Child Reordering

For **Composite** and **Sequence** nodes, child nodes can be reordered visually using the Up/Down buttons next to each child:

- The index label shows the child’s current position in the composite node  
- Clicking **↑** moves the child up in the sequence  
- Clicking **↓** moves the child down in the sequence  
- These changes are automatically reflected in the underlying `CompositeNode` data  

---

## License

MIT License — free to use, modify, and distribute.
