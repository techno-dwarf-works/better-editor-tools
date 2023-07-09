using System;
using System.Reflection;
using Better.EditorTools.Comparers;
using Better.EditorTools.Helpers.Caching;
using Better.EditorTools.Utilities;
using Better.Extensions.Runtime;
using Better.Tools.Runtime.Attributes;
using UnityEditor;

namespace Better.EditorTools.Drawers.Base
{
    public abstract class MultiFieldDrawer<T> : FieldDrawer where T : UtilityWrapper
    {
        private static readonly Cache CacheField = new Cache();

        protected WrapperCollection<T> _wrappers;

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
            return _fieldInfo.GetFieldOrElementType();
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
            _wrappers.ValidateCachedProperties(CacheField, property, GetFieldOrElementType(), _attribute.GetType(), handler);
            return CacheField;
        }

        protected class Cache : Cache<WrapperCollectionValue<T>>
        {
        }

        protected MultiFieldDrawer(FieldInfo fieldInfo, MultiPropertyAttribute attribute) : base(fieldInfo, attribute)
        {
        }
    }
}