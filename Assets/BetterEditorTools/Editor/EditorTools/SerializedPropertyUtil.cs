using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Better.Extensions.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Better.EditorTools
{
    public class FieldInfoCache
    {
        public FieldInfoCache(FieldInfo fieldInfo, Type type)
        {
            FieldInfo = fieldInfo;
            Type = type;
        }

        public FieldInfo FieldInfo { get; }
        public Type Type { get; }
    }

    public static class SerializedPropertyUtil
    {
        [DidReloadScripts]
        private static void Reload()
        {
            FieldInfoFromPropertyPathCache.Clear();
        }

        private struct Cache : IEquatable<Cache>
        {
            private readonly Type _host;
            private readonly string _path;

            public Cache(Type host, string path)
            {
                _host = host;
                _path = path;
            }

            public bool Equals(Cache other)
            {
                return _host == other._host && string.Equals(_path, other._path);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Cache && Equals((Cache)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_host != null ? _host.GetHashCode() : 0) * 397) ^ (_path != null ? _path.GetHashCode() : 0);
                }
            }
        }

        private static readonly Dictionary<Cache, FieldInfoCache> FieldInfoFromPropertyPathCache = new Dictionary<Cache, FieldInfoCache>();

        public static Type GetScriptTypeFromProperty(this SerializedProperty property)
        {
            if (property.serializedObject.targetObject != null)
                return property.serializedObject.targetObject.GetType();

            // Fallback in case the targetObject has been destroyed but the property is still valid.
            var scriptProp = property.serializedObject.FindProperty("m_Script");

            if (scriptProp == null)
                return null;

            var script = scriptProp.objectReferenceValue as MonoScript;

            return script == null ? null : script.GetClass();
        }

        public static List<TAttributes> GetAttributes<TAttributes>(this SerializedProperty property, bool inherit = false) where TAttributes : Attribute
        {
            var fieldInfo = property.GetFieldInfoAndStaticTypeFromProperty();
            if (fieldInfo == null) return null;
            var attributes = fieldInfo.FieldInfo.GetCustomAttributes<TAttributes>(inherit);
            return attributes.ToList();
        }

        /// <summary>
        /// Returns the field info and field type for the property. The types are based on the
        /// static field definition.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static FieldInfoCache GetFieldInfoAndStaticTypeFromProperty(this SerializedProperty property)
        {
            var classType = GetScriptTypeFromProperty(property);
            if (classType == null)
            {
                return null;
            }

            var fieldPath = property.propertyPath;
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                // When the field we are trying to access is a dynamic instance, things are a bit more tricky
                // since we cannot "statically" (looking only at the parent class field types) know the actual
                // "classType" of the parent class.

                // The issue also is that at this point our only view on the object is the very limited SerializedProperty.

                // So we have to:
                // 1. try to get the FQN from for the current managed type from the serialized data,
                // 2. get the path *in the current managed instance* of the field we are pointing to,
                // 3. foward that to 'GetFieldInfoFromPropertyPath' as if it was a regular field,

                var objectTypename = property.managedReferenceFullTypename;
                GetTypeFromManagedReferenceFullTypeName(objectTypename, out classType);

                fieldPath = property.propertyPath;
            }

            if (classType == null)
            {
                return null;
            }

            return GetFieldInfoFromPropertyPath(classType, fieldPath);
        }

        /// <summary>
        /// Create a Type instance from the managed reference full type name description.
        /// The expected format for the typename string is the one returned by SerializedProperty.managedReferenceFullTypename.
        /// </summary>
        /// <param name="managedReferenceFullTypename"></param>
        /// <param name="managedReferenceInstanceType"></param>
        /// <returns></returns>
        public static bool GetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypename, out Type managedReferenceInstanceType)
        {
            managedReferenceInstanceType = null;

            var parts = managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                var assemblyPart = parts[0];
                var nsClassnamePart = parts[1];
                managedReferenceInstanceType = Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return managedReferenceInstanceType != null;
        }


        public static FieldInfoCache GetFieldInfoFromPropertyPath(Type host, string path)
        {
            var cache = new Cache(host, path);

            if (FieldInfoFromPropertyPathCache.TryGetValue(cache, out var fieldInfoCache))
            {
                return fieldInfoCache;
            }

            const string arrayData = @"\.Array\.data\[[0-9]+\]";
            // we are looking for array element only when the path ends with Array.data[x]
            var lookingForArrayElement = Regex.IsMatch(path, arrayData + "$");
            // remove any Array.data[x] from the path because it is prevents cache searching.
            path = Regex.Replace(path, arrayData, ".___ArrayElement___");

            FieldInfo fieldInfo = null;
            var type = host;
            var parts = path.Split('.');
            for (var i = 0; i < parts.Length; i++)
            {
                var member = parts[i];
                // GetField on class A will not find private fields in base classes to A,
                // so we have to iterate through the base classes and look there too.
                // Private fields are relevant because they can still be shown in the Inspector,
                // and that applies to private fields in base classes too.
                FieldInfo foundField = null;
                for (var currentType = type; foundField == null && currentType != null; currentType = currentType.BaseType)
                    foundField = currentType.GetField(member, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (foundField == null)
                {
                    FieldInfoFromPropertyPathCache.Add(cache, null);
                    return null;
                }

                fieldInfo = foundField;
                type = fieldInfo.FieldType;
                // we want to get the element type if we are looking for Array.data[x]
                if (i < parts.Length - 1 && parts[i + 1] == "___ArrayElement___" && type.IsArrayOrList())
                {
                    i++; // skip the "___ArrayElement___" part
                    type = type.GetArrayOrListElementType();
                }
            }

            // we want to get the element type if we are looking for Array.data[x]
            if (lookingForArrayElement && type != null && type.IsArrayOrList())
                type = type.GetArrayOrListElementType();

            fieldInfoCache = new FieldInfoCache(fieldInfo, type);
            FieldInfoFromPropertyPathCache.Add(cache, fieldInfoCache);
            return fieldInfoCache;
        }
    }
}