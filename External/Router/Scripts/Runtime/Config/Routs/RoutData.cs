using System;
using System.Collections.Generic;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router.Config
{
    [Serializable]
    public class RoutData : ScriptableEntity
    {
        [SerializeField] private AssetReference asset;
        [SerializeField] private RenderMode renderMode;
        [SerializeField] protected bool overrideSortOrder;
        [SerializeField] protected bool hidePrevRout = true;
        [SerializeField] protected int sortOrder;

        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.RoutDataEditorHelper.CreateNewArgData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.RoutDataEditorHelper.RemoveArgData(this, $removeElement)"
        )]
        [FoldoutGroup("Route Arguments")]
        [SerializeField] private List<RoutArgData> argsData;

        public const string AssetKey = nameof(asset);
        public const string ArgDataKey = nameof(argsData);

        // Public properties for editor access
        public List<RoutArgData> ArgsData
        {
            get => argsData;
            set => argsData = value;
        }

        public AddressableGameObject Asset => new(asset);
        public RenderMode RenderMode => renderMode;
        public bool OverrideSortOrder => overrideSortOrder;
        public bool HidePrevRout => hidePrevRout;
        public int SortOrder => sortOrder;
    }
}