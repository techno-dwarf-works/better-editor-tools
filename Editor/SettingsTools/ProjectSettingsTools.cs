﻿using System.IO;
using Better.Tools.Runtime;
using Better.Tools.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.SettingsTools
{
    public class ProjectSettingsTools<T> where T : ProjectSettings
    {
        private readonly string[] _folderPaths;
        public string ProjectSettingKey { get; }
        public string NamespacePrefix { get; }

        public ProjectSettingsTools(string namespacePrefix, string settingMenuItem)
        {
            NamespacePrefix = namespacePrefix;
            _folderPaths = new string[]
                { BetterEditorDefines.BetterPrefix, NamespacePrefix, BetterEditorDefines.ResourcesPrefix };
            var menuItemPrefix = $"{BetterEditorDefines.BetterPrefix}/{settingMenuItem}";
            ProjectSettingKey = $"{BetterEditorDefines.ProjectPrefix}/{menuItemPrefix}";
        }
        
        public ProjectSettingsTools(string namespacePrefix, string settingMenuItem, string[] settingsFolderNames)
        {
            NamespacePrefix = namespacePrefix;
            _folderPaths = settingsFolderNames;
            var menuItemPrefix = $"{BetterEditorDefines.BetterPrefix}/{settingMenuItem}";
            ProjectSettingKey = $"{BetterEditorDefines.ProjectPrefix}/{menuItemPrefix}";
        }

        private string GenerateResourcesRelativePath()
        {
            return Path.Combine(_folderPaths);
        }

        public T LoadOrCreateScriptableObject()
        {
            var name = typeof(T).Name;
            var settings = Resources.Load<T>(name);
            if (settings != null) return settings;

            settings = ScriptableObject.CreateInstance<T>();

            var relativePath = GenerateResourcesRelativePath();
            var absolutePath = Path.Combine(Application.dataPath, relativePath);

            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
            relativePath = Path.Combine("Assets", relativePath, $"{name}.asset");
            AssetDatabase.CreateAsset(settings, relativePath);
            return settings;
        }
    }
}