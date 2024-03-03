using System;
using Better.EditorTools.EditorAddons.Utilities;

namespace Better.EditorTools.EditorAddons.Drawers.Base
{
    public class WrapperCollectionValue<T> where T : UtilityWrapper
    {
        public WrapperCollectionValue(T wrapper, Type type)
        {
            Wrapper = wrapper;
            Type = type;
        }

        public T Wrapper { get; }
        public Type Type { get; }
    }
}