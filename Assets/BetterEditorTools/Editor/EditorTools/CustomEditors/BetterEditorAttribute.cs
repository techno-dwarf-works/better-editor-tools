using System;

namespace Better.EditorTools.CustomEditors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BetterEditorAttribute : Attribute
    {
        public Type EditorFor { get; }
        public bool EditorForChildClasses { get; }
        public bool OverrideDefaultEditor { get; set; }
        public int Order { get; set; }

        public BetterEditorAttribute(Type editorFor, bool editorForChildClasses)
        {
            EditorFor = editorFor;
            EditorForChildClasses = editorForChildClasses;
        }
    }
}