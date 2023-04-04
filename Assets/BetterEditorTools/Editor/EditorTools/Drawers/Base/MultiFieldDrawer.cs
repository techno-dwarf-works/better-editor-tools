using System;
using System.Collections.Generic;
using Better.EditorTools.Comparers;
using Better.EditorTools.Utilities;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.Drawers.Base
{
    public abstract class MultiFieldDrawer<T> : FieldDrawer where T : UtilityWrapper
    {
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
            var type = fieldInfo.FieldType;
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (type.IsGenericType && (genericTypeDefinition == typeof(List<>) || genericTypeDefinition.IsSubclassOf(typeof(List<>))))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        /// <summary>
        /// Validates if <see cref="_wrappers"/> contains property by <see cref="SerializedPropertyComparer"/>
        /// </summary>
        /// <param name="property">SerializedProperty what will be stored into <see cref="_wrappers"/></param>
        /// <param name="handler"><see cref="BaseUtility{THandler}"/> used to validate current stored wrappers and gets instance for recently added property</param>
        /// <typeparam name="THandler"></typeparam>
        /// <returns>Returns true if wrapper for <paramref name="property"/> was already stored into <see cref="_wrappers"/></returns>
        protected bool ValidateCachedProperties<THandler>(SerializedProperty property, BaseUtility<THandler> handler) where THandler : new()
        {
            var fieldType = GetFieldOrElementType();
            var contains = _wrappers.ContainsKey(property);
            if (contains)
            {
                handler.ValidateCachedProperties(_wrappers);
            }
            else
            {
                var gizmoWrapper = handler.GetUtilityWrapper<T>(fieldType, attribute.GetType());
                _wrappers.Add(property, new WrapperCollectionValue<T>(gizmoWrapper, fieldType));
            }

            return contains;
        }
    }
}