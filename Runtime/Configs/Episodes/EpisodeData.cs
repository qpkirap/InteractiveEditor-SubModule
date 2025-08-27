﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Component = Module.InteractiveEditor.Runtime.Component;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class EpisodeData : Component
    {
        [FoldoutGroup("Episode Images")]
        [LabelText("Images")]
        [ListDrawerSettings(
            ShowIndexLabels = true,
            DraggableItems = true,
            ShowItemCount = true,
            Expanded = true,
            ListElementLabelName = "title"
        )]
        [SerializeField] private List<ImageData> imageDatas = new();

        public IReadOnlyList<ImageData> ImageDatas => imageDatas;

        #region Editor

        public const string ImageDatasKey = nameof(imageDatas);

        #endregion
    }
}