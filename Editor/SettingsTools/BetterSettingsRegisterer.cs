﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Better.Extensions.Runtime;
using Better.Tools.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.SettingsTools
{
    public static class BetterSettingsRegisterer
    {
        public const string BetterPrefix = nameof(Better);
        public const string ProjectPrefix = "Project";
        public const string HighlightPrefix = "Highlight Settings";
        public const string ResourcesPrefix = nameof(Resources);

        [SettingsProviderGroup]
        internal static SettingsProvider[] CreateSettingsProvider()
        {
            var allInheritedType = typeof(BetterSettings).GetAllInheritedTypeWithUnityObjects();
            var types = allInheritedType
                .SelectMany(CollectionSelector).Where(ValidateInternal);
            var instances = types.Select(Activator.CreateInstance).Cast<SettingsProvider>();
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            return instances.ToArray();
        }

        private static IEnumerable<Type> CollectionSelector(Type x)
        {
            return typeof(BetterSettingsProvider<>).MakeGenericType(x).GetAllInheritedType();
        }

        private static bool ValidateInternal(Type type)
        {
            if (type.IsValueType)
            {
                return true;
            }

            var constructor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null);

            return constructor != null;
        }
    }
}