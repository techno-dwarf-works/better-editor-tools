using System;
using System.Diagnostics;
using Better.Tools.Runtime;

namespace Better.EditorTools.Attributes
{
    [Conditional(BetterEditorDefines.Editor)]
    [AttributeUsage(AttributeTargets.Class)]
    public class BetterCustomPropertyDrawer : Attribute
    {
        public Type ForAttribute { get; }
        public bool Override { get; set; }

        public BetterCustomPropertyDrawer(Type forAttribute)
        {
            ForAttribute = forAttribute;
        }
    }
}