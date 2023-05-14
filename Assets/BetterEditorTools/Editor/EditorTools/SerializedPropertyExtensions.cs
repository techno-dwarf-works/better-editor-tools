using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools
{
    public static class SerializedPropertyExtensions
    {
        private const BindingFlags FieldsBindingFlags =
            BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

        public static Type GetManagedType(this SerializedProperty property)
        {
#if UNITY_2021_1_OR_NEWER
            return property.managedReferenceValue?.GetType();
#else
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                return null;
            }

            var split = property.managedReferenceFullTypename.Split(' ');
            var assembly = GetAssembly(split[0]);
            var currentValue = assembly.GetType(split[1]);
            return currentValue;
#endif
        }

        private static Assembly GetAssembly(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static bool IsArrayElement(this SerializedProperty property)
        {
            return property.propertyPath.EndsWith("]");
        }

        public static int GetArrayIndex(this SerializedProperty p)
        {
            var matches = SerializedPropertyDefines.ArrayIndexRegex.Matches(p.propertyPath);
            if (matches.Count > 0)
            {
                if (int.TryParse(matches[matches.Count - 1].Name, out var result))
                {
                    return result;
                }
            }

            return -1;
        }

        public static string GetArrayNameFromPath(this SerializedProperty property)
        {
            return SerializedPropertyDefines.ArrayDataWithIndexRegex.Replace(property.propertyPath, "");
        }

        public static string GetArrayPath(this SerializedProperty property)
        {
            return SerializedPropertyDefines.ArrayRegex.Replace(property.propertyPath, "");
        }

        public static bool IsDisposed(this SerializedProperty property)
        {
            if (property == null || property.serializedObject == null)
            {
                return true;
            }

            var propertyPrtInfo = typeof(SerializedProperty).GetField("m_NativePropertyPtr", FieldsBindingFlags);
            var objectPrtInfo = typeof(SerializedObject).GetField("m_NativeObjectPtr", FieldsBindingFlags);
            try
            {
                if (propertyPrtInfo != null && objectPrtInfo != null)
                {
                    var propertyPrt = (IntPtr)propertyPrtInfo.GetValue(property);
                    var objectPrt = (IntPtr)objectPrtInfo.GetValue(property.serializedObject);
                    return propertyPrt == IntPtr.Zero || objectPrt == IntPtr.Zero;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        //https://gist.github.com/aholkner/214628a05b15f0bb169660945ac7923b

        /// <summary>
        /// Get the value of the serialized property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetValue(this SerializedProperty property)
        {
            string propertyPath = property.propertyPath;
            object value = property.serializedObject.targetObject;
            int i = 0;
            while (NextPathComponent(propertyPath, ref i, out var token))
                value = GetPathComponentValue(value, token);
            return value;
        }

        /// <summary>
        /// Set the value of the serialized property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetValue(this SerializedProperty property, object value)
        {
            Undo.RecordObject(property.serializedObject.targetObject, $"Set {property.name}");

            SetValueNoRecord(property, value);

            EditorUtility.SetDirty(property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Set the value of the serialized property, but do not record the change.
        /// The change will not be persisted unless you call SetDirty and ApplyModifiedProperties.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetValueNoRecord(this SerializedProperty property, object value)
        {
            string propertyPath = property.propertyPath;
            object container = property.serializedObject.targetObject;

            int i = 0;
            NextPathComponent(propertyPath, ref i, out var deferredToken);
            while (NextPathComponent(propertyPath, ref i, out var token))
            {
                container = GetPathComponentValue(container, deferredToken);
                deferredToken = token;
            }

            Debug.Assert(!container.GetType().IsValueType,
                $"Cannot use SerializedObject.SetValue on a struct object, as the result will be set on a temporary.  Either change {container.GetType().Name} to a class, or use SetValue with a parent member.");
            SetPathComponentValue(container, deferredToken, value);
        }

        /// <summary>
        /// Union type representing either a property name or array element index.  The element
        /// index is valid only if propertyName is null.
        /// </summary>
        private struct PropertyPathComponent
        {
            public string propertyName;
            public int elementIndex;
        }


        /// <summary>
        /// Parse the next path component from a SerializedProperty.propertyPath.  For simple field/property access,
        /// this is just tokenizing on '.' and returning each field/property name.  Array/list access is via
        /// the pseudo-property "Array.data[N]", so this method parses that and returns just the array/list index N.
        /// Call this method repeatedly to access all path components.  For example:
        ///      string propertyPath = "quests.Array.data[0].goal";
        ///      int i = 0;
        ///      NextPropertyPathToken(propertyPath, ref i, out var component);
        ///          => component = { propertyName = "quests" };
        ///      NextPropertyPathToken(propertyPath, ref i, out var component) 
        ///          => component = { elementIndex = 0 };
        ///      NextPropertyPathToken(propertyPath, ref i, out var component) 
        ///          => component = { propertyName = "goal" };
        ///      NextPropertyPathToken(propertyPath, ref i, out var component) 
        ///          => returns false
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="index"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        private static bool NextPathComponent(string propertyPath, ref int index, out PropertyPathComponent component)
        {
            component = new PropertyPathComponent();

            if (index >= propertyPath.Length)
                return false;

            var arrayElementMatch = SerializedPropertyDefines.ArrayElementRegex.Match(propertyPath, index);
            if (arrayElementMatch.Success)
            {
                index += arrayElementMatch.Length + 1; // Skip past next '.'
                component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
                return true;
            }

            int dot = propertyPath.IndexOf('.', index);
            if (dot == -1)
            {
                component.propertyName = propertyPath.Substring(index);
                index = propertyPath.Length;
            }
            else
            {
                component.propertyName = propertyPath.Substring(index, dot - index);
                index = dot + 1; // Skip past next '.'
            }

            return true;
        }

        private static object GetPathComponentValue(object container, PropertyPathComponent component)
        {
            if (component.propertyName == null)
                return ((IList)container)[component.elementIndex];
            else
                return GetMemberValue(container, component.propertyName);
        }

        private static void SetPathComponentValue(object container, PropertyPathComponent component, object value)
        {
            if (component.propertyName == null)
                ((IList)container)[component.elementIndex] = value;
            else
                SetMemberValue(container, component.propertyName, value);
        }

        private static object GetMemberValue(object container, string name)
        {
            if (container == null)
                return null;
            var type = container.GetType();
            var members = type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < members.Length; ++i)
            {
                if (members[i] is FieldInfo field)
                    return field.GetValue(container);
                else if (members[i] is PropertyInfo property)
                    return property.GetValue(container);
            }

            return null;
        }

        private static void SetMemberValue(object container, string name, object value)
        {
            var type = container.GetType();
            var members = type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < members.Length; ++i)
            {
                if (members[i] is FieldInfo field)
                {
                    field.SetValue(container, value);
                    return;
                }
                else if (members[i] is PropertyInfo property)
                {
                    property.SetValue(container, value);
                    return;
                }
            }

            Debug.Assert(false, $"Failed to set member {container}.{name} via reflection");
        }
    }
}