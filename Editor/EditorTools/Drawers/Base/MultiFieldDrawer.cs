using System;
using Better.EditorTools.Comparers;
using Better.EditorTools.Utilities;
using Better.Extensions.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Better.EditorTools.Drawers.Base
{
    public abstract class MultiFieldDrawer<T> : FieldDrawer where T : UtilityWrapper
    {
        private static readonly Cache CacheField = new Cache();
        
        protected class Cache
        {
            public bool IsValid { get; private set; }
            public WrapperCollectionValue<T> Value { get; private set; }

            public Cache Set(bool isValid, WrapperCollectionValue<T> value)
            {
                IsValid = isValid;
                Value = value;
                return this;
            }
        }

        protected WrapperCollection<T> _wrappers;

        /// <summary>
        /// Method generates explicit typed collection inherited from <see cref="WrapperCollection{T}"/> 
        /// </summary>
        /// <returns></returns>
        protected abstract WrapperCollection<T> GenerateCollection();

        protected override void Deconstruct()
        {
            _wrappers.Deconstruct();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _wrappers ??= GenerateCollection();
            base.OnGUI(position, property, label);
        }

        protected virtual Type GetFieldOrElementType()
        {
            if (fieldInfo.IsArrayOrList())
                return fieldInfo.GetArrayOrListElementType();

            return fieldInfo.FieldType;
        }


        /// <summary>
        /// Validates if <see cref="_wrappers"/> contains property by <see cref="SerializedPropertyComparer"/>
        /// </summary>
        /// <param name="property">SerializedProperty what will be stored into <see cref="_wrappers"/></param>
        /// <param name="handler"><see cref="BaseUtility{THandler}"/> used to validate current stored wrappers and gets instance for recently added property</param>
        /// <typeparam name="THandler"></typeparam>
        /// <returns>Returns true if wrapper for <paramref name="property"/> was already stored into <see cref="_wrappers"/></returns>
        protected Cache ValidateCachedProperties<THandler>(SerializedProperty property, BaseUtility<THandler> handler)
            where THandler : new()
        {
            var fieldType = GetFieldOrElementType();
            if (_wrappers.TryGetValue(property, out var wrapperCollectionValue))
            {
                handler.ValidateCachedProperties(_wrappers);
                return CacheField.Set(true, wrapperCollectionValue);
            }

            var wrapper = handler.GetUtilityWrapper<T>(fieldType, attribute.GetType());
            if (wrapper == null)
            {
                return CacheField.Set(false, null);
            }

            var collectionValue = new WrapperCollectionValue<T>(wrapper, fieldType);
            _wrappers.Add(property, collectionValue);

            return CacheField.Set(false, collectionValue);
        }
    }
}