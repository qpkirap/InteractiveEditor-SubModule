using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router.Routs
{
    public class RouterController : MonoBehaviour
    {
        [SerializeField] private GameObject lockUserControlArea;
        [SerializeField] private Transform routContainerParent;
        [Space]
        [SerializeField] private string uiCameraTag = "UI Camera";
        
        private readonly Dictionary<RoutKey, RoutContainer> routsDict = new();
        private readonly Dictionary<RoutKey, AddressableGameObject> routAssetsDict = new();
        
        private Camera uiCamera;
        
        private Camera UICamera => uiCamera ??= GameObject.FindWithTag(uiCameraTag)?.GetComponent<Camera>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public async UniTask<(RoutContainer rout, bool isNewLoad)> GetOrCreateContainer(RoutKey routKey, RoutData routData, int sortOrder)
        {
            if (!routsDict.TryGetValue(routKey, out var rout))
            {
                var routAsset = routData.Asset;

                routAsset.SetParent(routContainerParent);

                var routGO = await routAsset.LoadAsync();

                rout = routGO.GetComponent<RoutContainer>();

                SetupCamera(rout, routData);
                
                routsDict[routKey] = rout;
                routAssetsDict[routKey] = routAsset;
                
                rout.SetSortOrder(sortOrder);
                
                return (rout, true);
            }


            return (rout, false);
        }
        
        public void SetUserControlState(bool state)
        {
            lockUserControlArea.SetActive(!state);
        }
        
        private void SetupCamera(RoutContainer container, RoutData routData)
        {
            var canvas = container.Canvas;

            canvas.renderMode = routData.RenderMode;
            
            switch (routData.RenderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    canvas.worldCamera = UICamera;
                    break;
            }
        }
        
        public async UniTask UnloadRouts()
        {
            var releaseTasks = routAssetsDict.Values
                .Select(x => x.ReleaseAsync());

            await UniTask.WhenAll(releaseTasks);

            routsDict.Clear();
            routAssetsDict.Clear();
        }
        
        public void Dispose()
        {
            foreach (var routAsset in routAssetsDict.Values)
            {
                routAsset.Dispose();
            }

            routsDict.Clear();
            routAssetsDict.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}