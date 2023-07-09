using UnityEngine;

namespace Better.EditorTools.Helpers
{
    public class HideGroup : GUI.Scope
    {
        private readonly Color _color;
        private readonly bool _wasEnabled;

        public HideGroup(bool satisfied)
        {
            _color = GUI.color;
            if (satisfied)
            {
                GUI.color = Color.clear;
                GUI.enabled = false;
            }
            _wasEnabled = GUI.enabled;
        }

        protected override void CloseScope()
        {
            GUI.color = _color;
            GUI.enabled = _wasEnabled;
        }
    }
}