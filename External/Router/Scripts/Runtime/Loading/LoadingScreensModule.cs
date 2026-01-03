using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Managers.Router
{
    public class LoadingScreensModule
    {
        private const string controllerName = "--- Loading Screen Controller ---";

        [Inject] private readonly RouterConfig config;

        private LoadingScreensController controller;
        private AddressableGameObject controllerAsset;

        private LoadingScreenKey currentKey;
        
        private readonly List<UniTaskCompletionSource> tasks = new();
        private readonly List<LoadingScreenKey> currentKeys = new();
        
        public bool IsLoadingActive { get; private set; }
        
        public async UniTask Init()
        {
            controllerAsset = config.LoadingScreenControllerAsset;

            var containerGO = await controllerAsset.LoadAsync();

            controller = containerGO.GetComponent<LoadingScreensController>();
            controller.name = controllerName;
        }

        /// <summary>
        /// Показать экран загрузки
        /// </summary>
        /// <param name="screenKey">Ключ экрана загрузки. Доступ к ключам через LoadingScreenKeys</param>
        public async UniTask Show(LoadingScreenKey screenKey, bool useAnimation = true)
        {
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource();
            
            tasks.Add(item);

            await UniTask.WhenAll(currentTasks);
            
            await ShowProcess(screenKey, useAnimation).ContinueWith(() =>
            {
                item.TrySetResult();
                
                return tasks.Remove(item);
            });
        }

        /// <summary>
        /// Скрыть экран загрузки
        /// </summary>
        public async UniTask Hide(LoadingScreenKey screenKey)
        {
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource();
            
            tasks.Add(item);

            await UniTask.WhenAll(currentTasks);
            
            await HideProcess(screenKey).ContinueWith(() =>
            {
                item.TrySetResult();
                
                return tasks.Remove(item);
            });
        }
        
        public async UniTask Cancel()
        {
            foreach (var key in currentKeys)
            {
                var screen = await controller.GetScreen(key);

                if (screen == null) return;

                await screen.Fade(0f, cancel: true);

                screen.gameObject.SetActive(false);

                currentKeys.Remove(key);

                IsLoadingActive = false;
            }
        }

        public void Dispose()
        {
            controller.Dispose();
            controllerAsset.Dispose();
        }
        
        /// <summary>
        /// Показать экран загрузки
        /// </summary>
        /// <param name="screenKey">Ключ экрана загрузки. Доступ к ключам через LoadingScreenKeys</param>
        private async UniTask ShowProcess(LoadingScreenKey screenKey, bool useAnimation = true)
        {
            if (screenKey == null || string.IsNullOrEmpty(screenKey)) return;
            
            if (currentKeys.Contains(screenKey))
            {
                currentKeys.Add(screenKey);
                
                return;
            }

            currentKeys.Add(screenKey);

            var screenData = config.GetLoadingScreen(screenKey);
            var screen = await controller.GetOrCreateScreen(screenKey, screenData);

            screen.gameObject.SetActive(true);

            IsLoadingActive = true;

            if (screen == null)
            {
                return;
            }

            await screen.Fade(1f, !useAnimation);
        }
        
        public async UniTask HideProcess(LoadingScreenKey key = null)
        {
            var currentKey = key;

            if (key == null)
            {
                currentKey = currentKeys.LastOrDefault();
            }

            if (currentKey == null)
            {
                return;
            }

            var checkCount = currentKeys.Count(x => x.Equals(currentKey));

            if (checkCount > 1)
            {
                currentKeys.Remove(currentKey);
                
                return;
            }

            var screen = await controller.GetScreen(currentKey);

            if (screen == null)
            {
                return;
            }

            await screen.Fade(0f);

            screen.gameObject.SetActive(false);

            currentKeys.Remove(currentKey);

            IsLoadingActive = false;
        }
    }
}