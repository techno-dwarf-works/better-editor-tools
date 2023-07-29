using System.Reflection;
using Better.Tools.Runtime;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.Helpers
{
    public static class EditorGUIHelpers
    {
        private static MethodInfo _defaultPropertyField;
        public static bool PropertyFieldSafe(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_defaultPropertyField == null)
            {
                var type = typeof(EditorGUI);
                _defaultPropertyField = type.GetMethod("DefaultPropertyField", BetterEditorDefines.MethodFlags);
            }

            return (bool)_defaultPropertyField.Invoke(null, new object[] { position, property, label });
        }
    }
}