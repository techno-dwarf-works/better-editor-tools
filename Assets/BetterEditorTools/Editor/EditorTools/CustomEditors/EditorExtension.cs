﻿using UnityEditor;
using UnityEngine;

namespace Better.EditorTools.CustomEditors
{
    public abstract class EditorExtension
    {
        private protected readonly Object _target;
        private protected readonly SerializedObject _serializedObject;

        protected EditorExtension(Object target, SerializedObject serializedObject)
        {
            _target = target;
            _serializedObject = serializedObject;
        }

        public abstract void OnDisable();
        public abstract void OnEnable();
        public abstract void OnInspectorGUI();
        public abstract void OnChanged();
    }
}