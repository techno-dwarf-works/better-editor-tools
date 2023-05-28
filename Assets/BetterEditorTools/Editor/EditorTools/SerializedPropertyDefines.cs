using System.Reflection;
using System.Text.RegularExpressions;

namespace Better.EditorTools
{
    public static class SerializedPropertyDefines
    {
        public static readonly Regex ArrayDataWithIndexRegex = new Regex(@"\.Array\.data\[[0-9]+\]", RegexOptions.Compiled);

        public static readonly Regex ArrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);
        public static readonly Regex ArrayIndexRegex = new Regex(@"\[([^\[\]]*)\]", RegexOptions.Compiled);
        
        public static readonly Regex ArrayRegex = new Regex(@"\.Array\.data", RegexOptions.Compiled);
        
        
        public const BindingFlags FieldsBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public const int IteratorNotAtEnd = 2;
    }
}