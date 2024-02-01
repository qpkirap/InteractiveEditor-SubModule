﻿using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using Module.Utils.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    [CreateAssetMenu(menuName = "Configs/StoryConfigs", fileName = "StoryConfigs")]
    public class StoryConfigs : BaseConfig
    {
        [SerializeField] private List<StoryConfig> storyObjects = new();
        
        public IReadOnlyList<StoryConfig> StoryObjects => storyObjects;
    }

    [Serializable]
    public class StoryConfig
    {
        [field: SerializeField] private StoryObject storyObject;
        
        public StoryObject StoryObject => storyObject;
    }
}