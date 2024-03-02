using System.Reflection;
using Better.Tools.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.Drawers.Base
{
    public abstract class FieldDrawer
    {
        protected readonly FieldInfo _fieldInfo;
        protected readonly MultiPropertyAttribute _attribute;
        protected FieldDrawer _nextDrawer;

        protected FieldDrawer(FieldInfo fieldInfo, MultiPropertyAttribute attribute)
        {
            Selection.selectionChanged += OnSelectionChanged;
            _fieldInfo = fieldInfo;
            _attribute = attribute;
        }

        ~FieldDrawer()
        {
            EditorApplication.update += DeconstructOnMainThread;
        }

        public virtual void Initialize(FieldDrawer drawer)
        {
            _nextDrawer = drawer;
        }

        private void DeconstructOnMainThread()
        {
            EditorApplication.update -= DeconstructOnMainThread;
            Selection.selectionChanged -= OnSelectionChanged;
            Deconstruct();
        }

        private void OnSelectionChanged()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Deconstruct();
        }

        protected abstract void Deconstruct();

        protected virtual void DrawField(Rect position, SerializedProperty property, GUIContent label)
        {
            var buffer = PreparePropertyRect(position);
            EditorGUI.PropertyField(buffer, property, label, true);
        }

        internal void DrawFieldInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_nextDrawer != null)
            {
                _nextDrawer.DrawFieldInternal(position, property, label);
                return;
            }

            DrawField(position, property, label);
        }

        internal bool PreDrawInternal(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (_nextDrawer != null)
            {
                return PreDraw(ref position, property, label) && _nextDrawer.PreDrawInternal(ref position, property, label);
            }

            return PreDraw(ref position, property, label);
        }

        internal void PostDrawInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            _nextDrawer?.PostDrawInternal(position, property, label);
            PostDraw(position, property, label);
        }

        internal HeightCacheValue GetPropertyHeightInternal(SerializedProperty property, GUIContent label)
        {
            var propertyHeight = GetPropertyHeight(property, label);
            if (_nextDrawer != null)
            {
                var heightInternal = _nextDrawer.GetPropertyHeightInternal(property, label);
                return propertyHeight + heightInternal;
            }

            return propertyHeight;
        }

        protected abstract bool PreDraw(ref Rect position, SerializedProperty property, GUIContent label);
        protected abstract Rect PreparePropertyRect(Rect original);
        protected abstract void PostDraw(Rect position, SerializedProperty property, GUIContent label);

        protected virtual HeightCacheValue GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return HeightCacheValue.GetFull(EditorGUI.GetPropertyHeight(property, label, true));
        }
    }
}