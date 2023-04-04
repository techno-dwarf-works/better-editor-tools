using System.Collections.Generic;
using Better.EditorTools.Comparers;
using Better.EditorTools.Utilities;
using UnityEditor;

namespace Better.EditorTools.Drawers.Base
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
            foreach (var gizmo in Values)
            {
                gizmo.Wrapper.Deconstruct();
            }
        }
    }
}