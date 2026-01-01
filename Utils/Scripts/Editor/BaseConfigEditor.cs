using System;
using Module.Utils.Configs;
using UnityEditor;
using UnityEngine;

namespace Module.Utils.Editor
{
    [CustomEditor(typeof(BaseConfig))]
    public class BaseConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty idProperty;

        public virtual void OnEnable()
        {
            idProperty = serializedObject.FindProperty("id");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var idCache = idProperty.stringValue;
            
            EditorGUILayout.TextField(idCache);

            if (GUILayout.Button("Generate Id"))
            {
                GenerateId();
            }
        }

        private void GenerateId()
        {
            if (string.IsNullOrEmpty(idProperty.stringValue))
            {
                idProperty.stringValue = Guid.NewGuid().ToString("N");
                
                EditorUtility.SetDirty(target);
                
                serializedObject.ApplyModifiedProperties();
                
                Debug.Log("Id: " + idProperty.stringValue);
            }
        }
    }
}