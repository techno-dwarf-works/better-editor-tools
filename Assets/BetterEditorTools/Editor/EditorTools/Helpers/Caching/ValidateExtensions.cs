using System;
using Better.EditorTools.Drawers.Base;
using Better.EditorTools.Utilities;
using UnityEditor;

namespace Better.EditorTools.Helpers.Caching
{
    public static class ValidateCachedPropertiesExtensions
    {
        public static void ValidateCachedProperties<TCache, TWrapper, THandler>(this
                WrapperCollection<TWrapper> wrappers, TCache cache, SerializedProperty property, Type fieldType,
            Type attributeType, BaseUtility<THandler> handler) where TCache : Cache<WrapperCollectionValue<TWrapper>>
            where TWrapper : UtilityWrapper
            where THandler : new()
        {
            if (wrappers.TryGetValue(property, out var wrapperCollectionValue))
            {
                handler.ValidateCachedProperties(wrappers);
                cache.Set(true, wrapperCollectionValue);
                return;
            }

            var wrapper = handler.GetUtilityWrapper<TWrapper>(fieldType, attributeType);
            if (wrapper == null)
            {
                cache.Set(false, null);
                return;
            }

            var collectionValue = new WrapperCollectionValue<TWrapper>(wrapper, fieldType);
            wrappers.Add(property, collectionValue);
            cache.Set(false, collectionValue);
        }
    }
}