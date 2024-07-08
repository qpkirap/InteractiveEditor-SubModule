using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseData<TComponent> : ScriptableObject, IConfigData
        where TComponent : IBaseComponent
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField, HorizontalGroup, ValueDropdown(nameof(GetPrefixNames)), LabelText("Title")] public string Prefix { get; private set; }
        [field: SerializeField, HorizontalGroup, LabelText("")] public string Title { get; private set; }
        [field: SerializeField, SerializeReference, TabGroup("Компоненты"), ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "GetTitle"), OnValueChanged(nameof(OnComponentsChanged))]
        public List<TComponent> Components { get; private set; } = new();
#if UNITY_EDITOR
        public abstract List<TComponent> DefaultComponents { get; }
#endif

        
        public IEnumerable<IBaseComponent> GetComponents()
        {
            return Components.Select(item => item as IBaseComponent);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T GetComponent<T>()
            where T : class
        {
            foreach (var compnent in Components)
            {
                if (compnent is T tcompnent)
                {
                    return tcompnent;
                }
            }

            return null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool HasComponent<T>()
            where T : class
        {
            foreach (var compnent in Components)
            {
                if (compnent is T)
                {
                    return true;
                }
            }

            return false;
        }
        
        protected virtual IEnumerable GetPrefixNames()
        {
            return new ValueDropdownList<string>()
            {
            };
        }
        
        protected virtual void OnComponentsChanged()
        {
            GenerateIdComponents(false);
        }
        
        protected virtual void GenerateIdComponents(bool force = false)
        {
            if (Components == null) return;
            
            foreach (var baseComponent in Components)
            {
                if (baseComponent is BaseComponent component)
                {
                    GenerateIdComponent(component, force);
                }
            }
        }
        
        protected void GenerateIdComponent(IBaseComponent component, bool force = false)
        {
            if (component is BaseComponent baseComponent)
            {
                baseComponent.GenerateId(force);
            }

            if (component is IReferenceComponent referenceComponent)
            {
                referenceComponent.GetReferences().ForEach(item => GenerateIdComponent(item, force));
            }
        }
        
#if UNITY_EDITOR
        [Button("GenerateIdComponents")]
        internal void GenerateIdsComponentsEditor()
        {
            GenerateIdComponents();
        }
        
        [Button("Generate Id")]
        internal void GenerateId(bool force = false)
        {
            Id = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            
            GenerateIdComponents(force);

            var path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(path, $"Data_{Id}");
        }
        
        [Button("Reset To Default Components")]
        internal void ResetToDefaultComponents()
        {
            Components.Clear();

            foreach (var defaultComponent in DefaultComponents)
            {
                Components.Add(defaultComponent);
            }

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