﻿using System;
using System.Diagnostics;
using Better.Tools.Runtime;

namespace Better.EditorTools.Attributes
{
    [Conditional(BetterEditorDefines.Editor)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MultiCustomPropertyDrawer : Attribute
    {
        public Type ForAttribute { get; }
        public bool Override { get; set; }

        public MultiCustomPropertyDrawer(Type forAttribute)
        {
            ForAttribute = forAttribute;
        }
    }
}