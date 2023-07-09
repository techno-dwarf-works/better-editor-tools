using System;
using System.Diagnostics;
using UnityEngine;

namespace Better.Tools.Runtime.Attributes
{
    [Conditional(BetterEditorDefines.Editor)]
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class MultiPropertyAttribute : PropertyAttribute
    {
        
    }
}