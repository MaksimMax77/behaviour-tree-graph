using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeGraph.Editor.NodeViews
{
    public class CompositeChildOrderView
    {
        public event Action<bool> MoveIndexButtonClicked;
        private readonly VisualElement _root;
        private VisualElement _container;
        private Label _indexLabel;
        private Button _upButton;
        private Button _downButton;

        public CompositeChildOrderView(VisualElement parent)
        {
            _root = parent;
            CreateUI();
            Hide();
        }

        private void CreateUI()
        {
            _container = new VisualElement();
            _container.style.flexDirection = FlexDirection.Row;
            _container.style.alignItems = Align.Center;
            _container.style.marginTop = 4;
            
            _upButton = new Button { text ="↑" };
            _downButton = new Button{ text = "↓" };
            
            _upButton.clicked += OnUpButtonClicked;
            _downButton.clicked += OnDownButtonClicked;

            _upButton.style.width = 20;
            _downButton.style.width = 20;

            _indexLabel = new Label("0")
            {
                style =
                {
                    minWidth = 20,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };

            _container.Add(_upButton);
            _container.Add(_indexLabel);
            _container.Add(_downButton);

            _root.Add(_container);
        }

        public void Dispose()//todo нужно где-то вызывать 
        {
            _upButton.clicked -= OnUpButtonClicked;
            _downButton.clicked -= OnDownButtonClicked;
        }

        private void OnUpButtonClicked()
        {
            MoveIndexButtonClicked?.Invoke(true);
        }

        private void OnDownButtonClicked()
        {
            MoveIndexButtonClicked?.Invoke(false);
        }

        public void SetIndex(int index)
        {
            _indexLabel.text = index.ToString();
        }

        public void Show()
        {
            _container.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _container.style.display = DisplayStyle.None;
        }
    }
}