using System.Reflection;
using Better.Internal.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.EditorAddons.Helpers
{
    public static class EditorGUIHelpers
    {
        private static MethodInfo _defaultPropertyField;

        static EditorGUIHelpers()
        {
            var type = typeof(EditorGUI);
            _defaultPropertyField = type.GetMethod("DefaultPropertyField", Defines.MethodFlags);
        }
        
        public static bool PropertyFieldSafe(Rect position, SerializedProperty property, GUIContent label)
        {
            return (bool)_defaultPropertyField.Invoke(null, new object[] { position, property, label });
        }
    }
}