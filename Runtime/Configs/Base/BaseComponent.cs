using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public abstract class BaseComponent : IBaseComponent
    {
        [field: SerializeField, ReadOnly] public string Id { get; protected set; }
        
        public void GenerateId(bool force = false)
        {
            if (!string.IsNullOrEmpty(Id) && !force) return;
            
            Id = Guid.NewGuid().ToString("N");

            if (this is IReferenceComponent referenceComponent)
            {
                foreach (var item in referenceComponent.GetReferences())
                {
                    if (item is BaseComponent baseComponent)
                    {
                        baseComponent.GenerateId(force);
                    }
                }
            }
        }
    }

    public interface IBaseComponent
    {
        public string Id { get; }
    }
    
    public interface IReferenceComponent
    {
        public IReadOnlyList<IBaseComponent> GetReferences();
    }
}