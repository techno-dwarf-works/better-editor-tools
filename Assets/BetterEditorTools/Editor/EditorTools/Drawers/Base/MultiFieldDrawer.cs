using System;
using System.Reflection;
using Better.EditorTools.EditorAddons.Comparers;
using Better.EditorTools.EditorAddons.Helpers.Caching;
using Better.EditorTools.EditorAddons.Utilities;
using Better.EditorTools.Runtime.Attributes;
using Better.Extensions.Runtime;
using UnityEditor;

namespace Better.EditorTools.EditorAddons.Drawers.Base
{
    public abstract class MultiFieldDrawer<T> : FieldDrawer where T : UtilityWrapper
    {
        private static readonly CacheValue CacheValueField = new CacheValue();

        protected WrapperCollection<T> _wrappers;

        protected class CacheValue : CacheValue<WrapperCollectionValue<T>>
        {
        }

        protected MultiFieldDrawer(FieldInfo fieldInfo, MultiPropertyAttribute attribute) : base(fieldInfo, attribute)
        {
        }

        /// <summary>
        /// Method generates explicit typed collection inherited from <see cref="WrapperCollection{T}"/> 
        /// </summary>
        /// <returns></returns>
        protected abstract WrapperCollection<T> GenerateCollection();

        public override void Initialize(FieldDrawer drawer)
        {
            base.Initialize(drawer);
            _wrappers = GenerateCollection();
        }

        protected override void Deconstruct()
        {
            _wrappers.Deconstruct();
        }

        protected virtual Type GetFieldOrElementType()
        {
            var fieldType = _fieldInfo.FieldType;
            if (fieldType.IsArrayOrList())
                return fieldType.GetCollectionElementType();

            return fieldType;
        }

        /// <summary>
        /// Validates if <see cref="_wrappers"/> contains property by <see cref="SerializedPropertyComparer"/>
        /// </summary>
        /// <param name="property">SerializedProperty what will be stored into <see cref="_wrappers"/></param>
        /// <param name="handler"><see cref="BaseUtility{THandler}"/> used to validate current stored wrappers and gets instance for recently added property</param>
        /// <typeparam name="THandler"></typeparam>
        /// <returns>Returns true if wrapper for <paramref name="property"/> was already stored into <see cref="_wrappers"/></returns>
        protected CacheValue ValidateCachedProperties<THandler>(SerializedProperty property, BaseUtility<THandler> handler)
            where THandler : new()
        {
            ValidateCachedPropertiesUtility.Validate(_wrappers, CacheValueField, property, GetFieldOrElementType(), _attribute.GetType(), handler);
            return CacheValueField;
        }
    }
}