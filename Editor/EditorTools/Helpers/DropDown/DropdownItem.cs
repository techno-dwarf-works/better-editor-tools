using System;
using UnityEngine;

namespace Better.EditorTools.Helpers.DropDown
{
    public class DropdownItem : DropdownBase
    {
        private readonly Action<object> _onSelect;
        private readonly object _object;

        public DropdownItem(GUIContent content, Action<object> onSelect, object data) : base(content)
        {
            _onSelect = onSelect;
            _object = data;
        }

        internal override bool Invoke(DropdownWindow downPopup)
        {
            _onSelect?.Invoke(_object);
            return true;
        }
    }
}