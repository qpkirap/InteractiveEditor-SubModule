﻿﻿﻿﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class BaseData : ScriptableObject, IConfigData
    {
        [ShowInInspector, ReadOnly, PropertyOrder(-1)]
        public string Id { get; private set; }
        
        [HorizontalGroup("Title", 0.3f), ValueDropdown(nameof(GetPrefixNames)), LabelText("Prefix")]
        [SerializeField] public string Prefix;
        
        [HorizontalGroup("Title", 0.7f), LabelText("Title")]
        [SerializeField] private string title;
        
        public string Title => title;
        
        [ShowInInspector, TabGroup("Components"), ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "@GetTitle()"), OnValueChanged(nameof(OnComponentsChanged))]
        [SerializeField, SerializeReference]
        private List<IBaseComponent> components = new();
        
        public List<IBaseComponent> Components => components;
        
#if UNITY_EDITOR
        public abstract List<IBaseComponent> DefaultComponents { get; }
#endif

        public IEnumerable<IBaseComponent> GetComponents() => Components ?? Enumerable.Empty<IBaseComponent>();

        public virtual T GetComponent<T>() where T : class => Components?.OfType<T>().FirstOrDefault();

        public virtual bool HasComponent<T>() where T : class => Components?.OfType<T>().Any() ?? false;
        
        protected virtual IEnumerable GetPrefixNames()
        {
            return new ValueDropdownList<string>();
        }
        
        private void OnComponentsChanged()
        {
            GenerateIdComponents(false);
        }
        
        private void GenerateIdComponents(bool force = false)
        {
            if (Components == null) return;
            
            foreach (var component in Components.OfType<BaseComponent>())
            {
                component.GenerateId(force);
                GenerateIdForReferences(component, force);
            }
        }
        
        private void GenerateIdForReferences(IBaseComponent component, bool force = false)
        {
            if (component is IReferenceComponent referenceComponent)
            {
                foreach (var reference in referenceComponent.GetReferences())
                {
                    if (reference is BaseComponent baseComponent)
                    {
                        baseComponent.GenerateId(force);
                        GenerateIdForReferences(reference, force);
                    }
                }
            }
        }
        
#if UNITY_EDITOR
        [FoldoutGroup("Development Tools", expanded: false)]
        [Button("Generate Component IDs", ButtonSizes.Small)]
        private void GenerateIdsComponentsEditor()
        {
            GenerateIdComponents(true);
            EditorUtility.SetDirty(this);
        }
        
        [FoldoutGroup("Development Tools")]
        [Button("Generate New ID", ButtonSizes.Small)]
        public void GenerateId(bool force = false)
        {
            Id = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            
            GenerateIdComponents(force);

            var path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, $"Data_{Id}");
            }
        }
        
        [FoldoutGroup("Development Tools")]
        [Button("Reset To Defaults", ButtonSizes.Small)]
        public void ResetToDefaultComponents()
        {
            Components.Clear();
            Components.AddRange(DefaultComponents);
            GenerateIdComponents(true);
            EditorUtility.SetDirty(this);
        }
#endif
    }

    public interface IConfigData
    {
        public string Id { get; }
        public string Title { get; }
        public T GetComponent<T>() where T : class;
        public bool HasComponent<T>() where T : class;
        public IEnumerable<IBaseComponent> GetComponents();
    }
}