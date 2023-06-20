using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.Drawers.Base
{
    public abstract class FieldDrawer : PropertyDrawer
    {
        protected FieldDrawer()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        ~FieldDrawer()
        {
            EditorApplication.update += DeconstructOnMainThread;
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

        /// <summary>
        /// Internal method called by Unity
        /// Execution order:
        /// <example>
        /// if(!<see cref="PreDraw"/>) -> <see cref="DrawField"/> -> <see cref="PostDraw"/>
        /// </example>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!PreDraw(ref position, property, label)) return;

            DrawField(position, property, label);

            PostDraw(position, property, label);
        }

        protected virtual void DrawField(Rect position, SerializedProperty property, GUIContent label)
        {
            var preparePropertyRect = PreparePropertyRect(position);
            EditorGUI.PropertyField(preparePropertyRect, property, label, true);
        }

        protected abstract bool PreDraw(ref Rect position, SerializedProperty property, GUIContent label);
        protected abstract Rect PreparePropertyRect(Rect original);
        protected abstract void PostDraw(Rect position, SerializedProperty property, GUIContent label);
    }
}