using System.Collections.Generic;
using Better.EditorTools.EditorAddons.Comparers;
using Better.EditorTools.EditorAddons.Utilities;
using UnityEditor;

namespace Better.EditorTools.EditorAddons.Drawers.Base
{
    public class WrapperCollection<T> : Dictionary<SerializedProperty, WrapperCollectionValue<T>>
        where T : UtilityWrapper
    {
        public WrapperCollection() : base(SerializedPropertyComparer.Instance)
        {
        }

        /// <summary>
        /// Deconstruct method for stored wrappers
        /// </summary>
        public void Deconstruct()
        {
            foreach (var value in Values)
            {
                value.Wrapper.Deconstruct();
            }
        }
    }
}