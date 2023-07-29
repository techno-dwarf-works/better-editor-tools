using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.Helpers
{
    public class HideGroup : EditorGUI.DisabledGroupScope
    {
        private readonly Color _color;

        public HideGroup(bool satisfied) : base(satisfied)
        {
            _color = GUI.color;
            if (satisfied)
            {
                GUI.color = Color.clear;
            }
        }

        protected override void CloseScope()
        {
            base.CloseScope();
            GUI.color = _color;
        }
    }
}