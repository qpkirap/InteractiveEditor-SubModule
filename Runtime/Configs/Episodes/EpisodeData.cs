using System;
using System.Collections.Generic;
using UnityEngine;
using Component = Module.InteractiveEditor.Runtime.Component;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class EpisodeData : Component
    {
        [SerializeField] private List<ImageData> imageDatas = new();

        #region Editor

        public const string ImageDatasKey = nameof(imageDatas);

        #endregion
    }
}