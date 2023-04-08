using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Better.EditorTools.Comparers
{
    public static class SerializedPropertyExtensions
    {
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
    }
}