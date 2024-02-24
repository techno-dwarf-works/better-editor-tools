using Better.Internal.Core.Runtime;
using UnityEditor;
using UnityEditorInternal;

namespace Better.EditorTools.Helpers
{
    public static class ReorderableListHelpers
    {
        private static System.Reflection.MethodInfo _repaintInspectors = null;

        //TODO: Need to find better way to refresh ReorderableList
        public static void RepaintAllInspectors(SerializedProperty property)
        {
            if (_repaintInspectors == null)
            {
                var inspWin = typeof(ReorderableList);
                _repaintInspectors = inspWin.GetMethod("InvalidateParentCaches", Defines.MethodFlags);
            }

            if (_repaintInspectors != null) _repaintInspectors.Invoke(null, new object[] { property.propertyPath });
        }
    }
}