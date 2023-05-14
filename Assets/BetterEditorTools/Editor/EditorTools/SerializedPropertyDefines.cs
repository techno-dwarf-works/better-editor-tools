using System.Text.RegularExpressions;

namespace Better.EditorTools
{
    public static class SerializedPropertyDefines
    {
        public static readonly Regex ArrayDataRegex = new Regex(@"\.Array\.data\[[0-9]+\]", RegexOptions.Compiled);

        public static readonly Regex ArrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);
    }
}