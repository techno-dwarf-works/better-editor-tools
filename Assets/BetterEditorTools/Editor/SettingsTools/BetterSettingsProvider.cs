using Better.Tools.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.SettingsTools
{
    public abstract class BetterSettingsProvider<T> : SettingsProvider where T : BetterSettings
    {
        protected readonly T _settings;
        protected readonly SerializedObject _settingsObject;
        private GUIStyle _style;
        private const int Space = 8;

        protected BetterSettingsProvider(BetterSettingsTools<T> tools, SettingsScope scope = SettingsScope.User)
            : base(tools.ProjectSettingKey, scope)
        {
            _settings = tools.LoadOrCreateScriptableObject();
            _settingsObject = new SerializedObject(_settings);
            label = tools.NamespacePrefix;
        }

        public override void OnGUI(string searchContext)
        {
            var style = CreateOrGetStyle();
            using (new EditorGUILayout.VerticalScope(style))
            {
                DrawGUI();
            }

            _settingsObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private GUIStyle CreateOrGetStyle()
        {
            if (_style != null)
            {
                return _style;
            }

            _style = new GUIStyle();
            _style.margin = new RectOffset(Space, Space, Space, Space);
            return _style;
        }

        protected abstract void DrawGUI();
    }
}