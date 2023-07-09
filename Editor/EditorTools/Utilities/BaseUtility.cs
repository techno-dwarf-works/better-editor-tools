using System;
using System.Collections.Generic;
using Better.EditorTools.Drawers.Base;
using UnityEditor;

namespace Better.EditorTools.Utilities
{
    public abstract class UtilityWrapper
    {
        public abstract void Deconstruct();
    }

    public abstract class BaseUtility<THandler> where THandler : new()
    {
        private static THandler _instance;

        protected HashSet<Type> _availableTypes;
        protected BaseWrappersTypeCollection _wrappersCollection;

        public static THandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new THandler();
                }

                return _instance;
            }
        }

        protected BaseUtility()
        {
            Construct();
        }

        private void Construct()
        {
            _wrappersCollection = GenerateCollection();
            _availableTypes = GenerateAvailable();
        }
        

        /// <summary>
        /// Type collection for <see cref="GetUtilityWrapper"/>.
        /// <example> Example:
        /// <code>
        /// return new WrappersTypeCollection()
        /// {
        ///     {
        ///         typeof(SupportedAttributeType), new ()
        ///         {
        ///             { typeof(SupportedType), typeof(WrapperForSupportedType) },
        ///             { typeof(SupportedType2), typeof(WrapperForSupportedType2) }
        ///         }
        ///     }
        /// };
        /// </code>
        /// </example>
        /// <seealso cref="GenerateAvailable"/>
        /// </summary>
        /// <returns></returns>
        protected abstract BaseWrappersTypeCollection GenerateCollection();

        /// <summary>
        /// Types collection for <see cref="IsSupported"/> checks
        /// <example> Example:
        /// <code>
        ///  return new HashSet(BaseComparer.Instance)
        ///  {
        ///     typeof(SupportedType),
        ///     typeof(SupportedType2)
        ///  };
        /// </code>
        /// </example>
        /// <seealso cref="GenerateCollection"/>
        /// </summary>
        /// <returns></returns>
        protected abstract HashSet<Type> GenerateAvailable();

        /// <summary>
        /// Generate ready to use wrapper's instance by dictionary from <see cref="GetWrapperDictionary"/>
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="attributeType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUtilityWrapper<T>(Type fieldType, Type attributeType) where T : UtilityWrapper
        {
            if (!IsSupported(fieldType))
            {
                return null;
            }

            if (_wrappersCollection.TryGetValue(attributeType, fieldType, out var wrapperType))
            {
                return (T)Activator.CreateInstance(wrapperType);
            }
            throw new KeyNotFoundException($"Supported types not found for {fieldType}");
        }

        /// <summary>
        /// Checks if type available in utility
        /// <seealso cref="GenerateAvailable"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsSupported(Type type)
        {
            return _availableTypes.Contains(type);
        }
    }

    public static class BaseUtilityExtension
    {
        /// <summary>
        /// Validates stored properties if their <see cref="WrapperCollectionValue{T}.Type"/> supported
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="gizmoWrappers"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        public static void ValidateCachedProperties<THandler, T>(this BaseUtility<THandler> handler, WrapperCollection<T> gizmoWrappers) where T : UtilityWrapper where THandler : new()
        {
            List<SerializedProperty> keysToRemove = null;
            foreach (var value in gizmoWrappers)
            {
                if (handler.IsSupported(value.Value.Type)) continue;
                if (keysToRemove == null)
                {
                    keysToRemove = new List<SerializedProperty>();
                }
                keysToRemove.Add(value.Key);
            }

            if (keysToRemove != null)
            {
                foreach (var serializedProperty in keysToRemove)
                {
                    gizmoWrappers.Remove(serializedProperty);
                }
            }
        }
    }
}