using System.Reflection;

namespace Better.Tools.Runtime
{
    public class BetterEditorDefines
    {
        public const string Editor = "UNITY_EDITOR";
        
        public const BindingFlags MethodFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                          BindingFlags.Static | BindingFlags.Instance |
                                          BindingFlags.DeclaredOnly;

        public const BindingFlags FieldsFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    }
}