using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using Managers.Router.Config.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router
{
    public class LoadingScreensController : MonoBehaviour, IDisposable
    {
        private readonly Dictionary<LoadingScreenKey, LoadingScreenContainer> screensDict = new();
        private readonly Dictionary<LoadingScreenKey, AddressableGameObject> screenAssetsDict = new();

        [SerializeField] private Transform container;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            FindStartAppScreen();
        }
        
        private void FindStartAppScreen()
        {
            var startAppScreen = FindObjectOfType<LoadingScreenContainer>();

            if (startAppScreen != null)
            {
                screensDict[LoadingScreenKeys.start] = startAppScreen;
            }
        }
        
        public async UniTask<LoadingScreenContainer> GetOrCreateScreen(LoadingScreenKey screenKey, LoadingScreenData screenData)
        {
            if (!screensDict.TryGetValue(screenKey, out var screen))
            {
                var screenAsset = screenData.Asset;

                screenAsset.SetParent(container);

                var screenGO = await screenAsset.LoadAsync();

                screen = screenGO.GetComponent<LoadingScreenContainer>();

                screensDict[screenKey] = screen;
                screenAssetsDict[screenKey] = screenAsset;
            }

            await screen.Init();

            return screen;
        }
        
        public async UniTask<LoadingScreenContainer> GetScreen(LoadingScreenKey screenKey)
        {
            return screensDict.TryGetValue(screenKey, out var screen)
                ? screen
                : null;
        }

        public void Dispose()
        {
            foreach (var screenAsset in screenAssetsDict.Values)
            {
                screenAsset.Dispose();
            }

            screensDict.Clear();
            screenAssetsDict.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}