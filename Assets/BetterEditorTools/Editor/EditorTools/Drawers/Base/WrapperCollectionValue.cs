using System;
using Better.EditorTools.Utilities;

namespace Better.EditorTools.Drawers.Base
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