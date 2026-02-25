using System;
using System.Reflection;
using BehaviorTreeGraph.Runtime.Core.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeGraph.Editor.NodeViews
{
    public class NodeInspectorRenderer
    {
        private readonly BehaviorTreeNode _behaviorTreeNode;

        public NodeInspectorRenderer(BehaviorTreeNode behaviorTreeNode)
        {
            _behaviorTreeNode = behaviorTreeNode;
        }

        public VisualElement CreateInspector()
        {
            var container = new VisualElement();

            var fields = GetSerializableFields(_behaviorTreeNode.GetType());

            foreach (var field in fields)
            {
                var fieldElement = CreateFieldElement(field, _behaviorTreeNode);
                if (fieldElement != null)
                    container.Add(fieldElement);
            }

            return container;
        }

        private FieldInfo[] GetSerializableFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private VisualElement CreateFieldElement(FieldInfo field, BehaviorTreeNode target)
        {
            var isPublic = field.IsPublic;
            var hasSerialize = field.GetCustomAttribute<SerializeField>() != null;

            if (!isPublic && !hasSerialize)
                return null;

            var fieldType = field.FieldType;
            var label = ObjectNames.NicifyVariableName(field.Name);

            if (fieldType == typeof(int))
                return CreateField<IntegerField, int>(label, field, target);

            if (fieldType == typeof(float))
                return CreateField<FloatField, float>(label, field, target);

            if (fieldType == typeof(bool))
                return CreateField<Toggle, bool>(label, field, target);

            if (fieldType == typeof(string))
                return CreateField<TextField, string>(label, field, target);

            if (fieldType.IsEnum)
                return CreateEnumField(label, field, target);

            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                return CreateObjectField(label, field, target, fieldType);

            return null;
        }

        private VisualElement CreateField<TField, TValue>(string label, FieldInfo field, BehaviorTreeNode target)
            where TField : BaseField<TValue>, new()
        {
            var uiField = new TField
            {
                label = label,
                value = (TValue)field.GetValue(target)
            };

            uiField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(target, "Change Node Field");
                field.SetValue(target, evt.newValue);
                EditorUtility.SetDirty(target);
            });

            return uiField;
        }

        private VisualElement CreateEnumField(string label, FieldInfo field, BehaviorTreeNode target)
        {
            var value = (Enum)field.GetValue(target);
            var enumField = new EnumField(label, value);

            enumField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(target, "Change Node Field");
                field.SetValue(target, evt.newValue);
                EditorUtility.SetDirty(target);
            });

            return enumField;
        }

        private VisualElement CreateObjectField(string label, FieldInfo field, BehaviorTreeNode target, Type objectType)
        {
            var objectField = new ObjectField(label)
            {
                objectType = objectType,
                value = (UnityEngine.Object)field.GetValue(target)
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(target, "Change Node Field");
                field.SetValue(target, evt.newValue);
                EditorUtility.SetDirty(target);
            });

            return objectField;
        }
    }
}