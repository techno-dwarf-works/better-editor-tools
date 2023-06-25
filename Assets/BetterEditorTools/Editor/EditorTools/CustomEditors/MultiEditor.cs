﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Better.Extensions.Runtime;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Better.EditorTools.CustomEditors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    internal sealed class MultiEditor : Editor
    {
        private List<EditorExtension> _preExtensions;
        private List<EditorExtension> _afterExtensions;
        private bool _overrideDefault;

        private void OnEnable()
        {
            try
            {
                if (target.IsNullOrDestroyed() || serializedObject.IsDisposed())
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            var targetType = target.GetType();

            var extensions = FindEditors(targetType);

            _preExtensions = new List<EditorExtension>();
            _afterExtensions = new List<EditorExtension>();
            Iterate(extensions);
        }

        private static IReadOnlyList<(Type type, BetterEditorAttribute)> FindEditors(Type targetType)
        {
            bool WherePredicate((Type type, BetterEditorAttribute attribute) x)
            {
                var att = x.Item2;
                if (att == null)
                {
                    return false;
                }

                if (att.EditorForChildClasses)
                {
                    return att.EditorFor.IsAssignableFrom(targetType);
                }

                return att.EditorFor == targetType;
            }

            return typeof(EditorExtension).GetAllInheritedType().Select(type => (type, type.GetCustomAttribute<BetterEditorAttribute>()))
                .Where(WherePredicate).OrderBy(x => x.Item2.Order).ToArray();
        }

        private void Iterate(IReadOnlyList<(Type type, BetterEditorAttribute)> extensions)
        {
            var paramArray = new object[2]
            {
                target, serializedObject
            };

            for (var index = 0; index < extensions.Count; index++)
            {
                var (type, betterEditorAttribute) = extensions[index];
                if (!_overrideDefault && betterEditorAttribute.OverrideDefaultEditor)
                {
                    _overrideDefault = true;
                }

                var extension = (EditorExtension)Activator.CreateInstance(type, paramArray);
                extension.OnEnable();
                if (betterEditorAttribute.Order < 0)
                {
                    _preExtensions.Add(extension);
                }
                else
                {
                    _afterExtensions.Add(extension);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                for (var i = 0; i < _preExtensions.Count; i++)
                {
                    _preExtensions[i].OnInspectorGUI();
                }

                if (!_overrideDefault)
                {
                    base.OnInspectorGUI();
                }

                for (var i = 0; i < _afterExtensions.Count; i++)
                {
                    _afterExtensions[i].OnInspectorGUI();
                }

                if (!change.changed) return;
                for (var i = 0; i < _preExtensions.Count; i++)
                {
                    _preExtensions[i].OnChanged();
                }

                for (var i = 0; i < _afterExtensions.Count; i++)
                {
                    _afterExtensions[i].OnChanged();
                }
            }
        }

        private void OnDisable()
        {
            for (var i = 0; i < _preExtensions.Count; i++)
            {
                _preExtensions[i].OnDisable();
            }

            for (var i = 0; i < _afterExtensions.Count; i++)
            {
                _afterExtensions[i].OnDisable();
            }
        }
    }
}