using BehaviorTreeGraph.Runtime.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeGraph.Editor
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        private BehaviorTreeView _graphView;
        private VisualElement _mainMenu;
        private Button _backButton;

        [MenuItem("AI/Behavior Tree Editor")]
        public static void Open()
        {
            var window = CreateInstance<BehaviorTreeEditorWindow>();
            window.titleContent = new GUIContent("Behavior Tree Editor");
            window.minSize = new Vector2(600, 400);
            
            var mainDisplay = Display.main;
            var centerX = mainDisplay.systemWidth / 2f - window.minSize.x / 2f;
            var centerY = mainDisplay.systemHeight / 2f - window.minSize.y / 2f;
            window.position = new Rect(centerX, centerY, window.minSize.x, window.minSize.y);
            window.Show();
        }

        private void OnEnable()
        {
            rootVisualElement.style.flexDirection = FlexDirection.Column;

            CreateGraphView();
            CreateBackButton();
            CreateMainMenu();
        }

        private void CreateGraphView()
        {
            _graphView = new BehaviorTreeView
            {
                style =
                {
                    flexGrow = 1,
                    display = DisplayStyle.None
                }
            };
            
            rootVisualElement.Add(_graphView);
        }

        private void CreateBackButton()
        {
            _backButton = new Button(() =>
                {
                    _graphView.style.display = DisplayStyle.None;
                    _mainMenu.style.display = DisplayStyle.Flex;
                    _backButton.style.display = DisplayStyle.None;
                })
                { text = "Back" };

            _backButton.style.height = 24;
            _backButton.style.alignSelf = Align.FlexStart;
            _backButton.style.display = DisplayStyle.None;

            rootVisualElement.Add(_backButton);
        }

        private void CreateMainMenu()
        {
            _mainMenu = new VisualElement();
            _mainMenu.style.flexDirection = FlexDirection.Column;
            _mainMenu.style.alignItems = Align.Center;
            _mainMenu.style.justifyContent = Justify.Center;
            _mainMenu.style.flexGrow = 1;
            _mainMenu.style.paddingTop = 50;

            var newButton = new Button(() => OpenGraphView(CreateNewBehaviorTree()))
            {
                text = "Create New Behavior Tree"
            };
            var openButton = new Button(() =>
                {
                    var asset = OpenExistingBehaviorTree();
                    if (asset != null)
                        OpenGraphView(asset);
                })
                { text = "Open Existing Behavior Tree" };
            
            StyleButton(newButton);
            StyleButton(openButton);

            _mainMenu.Add(newButton);
            _mainMenu.Add(openButton);

            rootVisualElement.Add(_mainMenu);
        }

        private void StyleButton(Button button)
        {
            button.style.width = 300;          
            button.style.height = 60;           
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.fontSize = 18;
            button.style.marginBottom = 10;    
        }

        private BehaviorTreeConfiguration CreateNewBehaviorTree()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Create New Behavior Tree",
                "NewBehaviorTree.asset",
                "asset",
                "Choose a location to save the new Behavior Tree"
            );

            if (string.IsNullOrEmpty(path)) return null;

            var asset = CreateInstance<BehaviorTreeConfiguration>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        private BehaviorTreeConfiguration OpenExistingBehaviorTree()
        {
            var path = EditorUtility.OpenFilePanel("Open Behavior Tree", "Assets", "asset");

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            path = FileUtil.GetProjectRelativePath(path);
            return AssetDatabase.LoadAssetAtPath<BehaviorTreeConfiguration>(path);
        }

        private void OpenGraphView(BehaviorTreeConfiguration asset)
        {
            if (asset == null)
            {
                return;
            }
            
            _graphView.SetTree(asset);
            _graphView.LoadNodesFromConfiguration();

            _mainMenu.style.display = DisplayStyle.None;
            _graphView.style.display = DisplayStyle.Flex;
            _backButton.style.display = DisplayStyle.Flex;
        }
    }
}