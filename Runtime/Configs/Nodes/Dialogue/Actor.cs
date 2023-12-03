using System;
using Module.Utils.Configs;
using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class Actor : ScriptableEntity
    {
        [SerializeField] LocalizedString name;
        
        public LocalizedString Name => name;
    }
}