using System.Reflection;
using UnityEngine;

namespace Better.Tools.Runtime
{
    public class BetterEditorDefines
    {
        public const string BetterPrefix = nameof(Better);
        public const string ProjectPrefix = "Project";
        public const string HighlightPrefix = "Highlight Settings";
        public const string ResourcesPrefix = nameof(Resources);
        public const string Editor = "UNITY_EDITOR";
        
        public const BindingFlags MethodFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                          BindingFlags.Static | BindingFlags.Instance |
                                          BindingFlags.DeclaredOnly;

        public const BindingFlags FieldsFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        
        public const BindingFlags ConstructorFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
    }
}