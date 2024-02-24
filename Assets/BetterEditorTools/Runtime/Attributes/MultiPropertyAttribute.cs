using System;
using System.Diagnostics;
using Better.Internal.Core.Runtime;
using UnityEngine;

namespace Better.Tools.Runtime.Attributes
{
    [Conditional(Defines.Editor)]
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class MultiPropertyAttribute : PropertyAttribute
    {
        
    }
}