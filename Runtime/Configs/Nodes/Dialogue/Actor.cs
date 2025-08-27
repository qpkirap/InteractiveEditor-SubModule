﻿using System;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class Actor : ScriptableEntity
    {
        [FoldoutGroup("Actor Info")]
        [LabelText("Actor Name")]
        [SerializeField] LocalizedString name;
        
        public LocalizedString Name => name;
    }
}