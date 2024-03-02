using System.Collections.Generic;
using Better.Extensions.EditorAddons;
using UnityEditor;

namespace Better.EditorTools.Comparers
{
    public class SerializedPropertyComparer : BaseComparer<SerializedPropertyComparer, SerializedProperty>,
        IEqualityComparer<SerializedProperty>
    {
        public bool Equals(SerializedProperty x, SerializedProperty y)
        {
            if (x.IsDisposed() || y.IsDisposed()) return false;
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.propertyPath == y.propertyPath;
        }

        public int GetHashCode(SerializedProperty obj)
        {
            return !obj.IsDisposed() && obj.propertyPath != null ? obj.propertyPath.GetHashCode() : 0;
        }
    }
}