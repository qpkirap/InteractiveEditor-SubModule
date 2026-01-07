using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using Managers.Router.Routs;
using Managers.Router.Scene;
using Module.InteractiveEditor.DI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Managers.Router
{
    public class Router : IRouter, IAsyncInitializable
    {
        private static readonly TimeSpan delay = TimeSpan.FromSeconds(.1f);
        
        private readonly ReactiveProperty<RoutContainer> currentRout = new();
        
        [Inject] private readonly RouterSettings settings;
        [Inject] private readonly IReadOnlyList<IRouterConfig> routerConfigs;
        [Inject] private readonly IObjectResolver resolver;
        
        private RouterModule routerModule;
        private RoutArgsModule routArgsModule;
        private ScenesModule scenesModule;
        private LoadingScreensModule loadingScreensModule;
        private UniTaskCompletionSource<bool> initTask;
        
        private bool backPressState;
        
        private readonly List<UniTaskCompletionSource<RoutContainer>> tasks = new();
        
        public IReadOnlyReactiveProperty<RoutContainer> CurrentRout => currentRout;
        public IReadOnlyCollection<RoutContainer> CurrentRouts => routerModule.Routs;
        
        // IInitializable - порядок инициализации (Router должен инициализироваться раньше других)
        public int InitOrder => 10;
        
        public async UniTask Init()
        {
            if (initTask != null) return;
            
            initTask = new();
            
            await PostInit().ContinueWith(() => initTask.TrySetResult(true));
        }

        private async UniTask PostInit()
        {
            loadingScreensModule = new();
            resolver.Inject(loadingScreensModule);
            
            await loadingScreensModule.Init();
            
            scenesModule = new();
            resolver.Inject(scenesModule);
            
            routerModule = new();
            resolver.Inject(routerModule);
            
            routArgsModule = new();
            
            await routerModule.Init();
        }
        
        public void SetUserControlState(bool state)
        {
            routerModule.SetUserControlState(state);
        }
        
        public async UniTask GoTo(
            RoutKey routKey,
            LoadingScreenKey loadingScreenKey = null,
            Func<bool> isActivateLoading = null,
            bool isFirstRout = false,
            bool useAnimation = true,
            params (string argKey, object data)[] routArgs)
        {
            await initTask.Task;
            
            SetUserControlState(false);
            
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource<RoutContainer>();
            
            tasks.Add(item);

            await UniTask.WhenAll(currentTasks);
            
            var isLoading = isActivateLoading != null && !string.IsNullOrEmpty(loadingScreenKey) && isActivateLoading();

            if (isLoading) await ShowLoadingScreen(loadingScreenKey);
            
            await GoToAsync(routKey, isFirstRout, useAnimation, routArgs).ContinueWith(x =>
            {
                item.TrySetResult(x);
                
                tasks.Remove(item);

                HideLoadingScreen(loadingScreenKey);
            });
            
            if (isLoading && !isActivateLoading()) await HideLoadingScreen(loadingScreenKey);
            
            if (currentRout.Value != null) await UniTask.WaitUntil(() => currentRout.Value.IsReady);

            if (!tasks.Any())
            {
                SetUserControlState(true);
            }
        }
        
        private async UniTask<RoutContainer> GoToAsync(
            RoutKey routKey,
            bool isFirstRout = false,
            bool useAnimation = true,
            params (string argKey, object data)[] routArgs)
        {
            SetBackPressState(false);

            // upd rout args
            routArgsModule.AddArgData(routKey, routArgs);

            // on hide
            var rout = await routerModule.GoToAsync(routKey, isFirstRout, useAnimation);

            // change current rout
            if (rout != null)
            {
                currentRout.SetValueAndForceNotify(rout);
            }
            else
            {
                routArgsModule.RemoveArgData(routKey);
            }

            //ReSubscribeBackPress();

            SetBackPressState(true);

            return rout;
        }
        
        public void SetBackPressState(bool state)
        {
            backPressState = state;
        }
        
        public async UniTask GoBack(bool ignoreBackPress = false, bool useAnimation = true)
        {
            await initTask.Task;
            
            if (!routerModule.IsAvailableBack) return;
            
            if (currentRout.Value != null)
            {
                routArgsModule.RemoveArgData(currentRout.Value.Data.Id);
            }
            
            SetUserControlState(false);
            
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource<RoutContainer>();
            
            tasks.Add(item);
            
            await UniTask.WhenAll(currentTasks);
            
            await routerModule.GoBack(ignoreBackPress, useAnimation).ContinueWith(routContainer =>
            {
                item.TrySetResult(routContainer);
                
                tasks.Remove(item);

                if (routContainer != null)
                {
                    currentRout.SetValueAndForceNotify(routContainer);
                }
            });

            if (currentRout.Value != null) await UniTask.WaitUntil(() => currentRout.Value.IsReady);
            
            if (!tasks.Any()) SetUserControlState(true);
        }

        public T GetRoutArgData<T>(string argKey)
        {
            return routArgsModule.GetRoutArgData<T>(argKey);
        }
        
        private async UniTask<bool> LoadScene(SceneKey sceneKey, LoadSceneMode sceneMode)
        {
            if (sceneMode == LoadSceneMode.Single)
            {
                await routerModule.UnloadRouts(disposeController: true);

                await UnloadUnusedAssets();
            }

            return await scenesModule.LoadScene(sceneKey, sceneMode);
        }

        public async UniTask<bool> LoadScene(
            SceneKey sceneKey,
            LoadingScreenKey loadingScreenKey = null,
            LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            await initTask.Task;
            
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource<RoutContainer>();
            
            tasks.Add(item);

            await UniTask.WhenAll(currentTasks);
            
            await ShowLoadingScreen(loadingScreenKey);

            var loadSceneResult = await LoadScene(sceneKey, sceneMode);

            // if (loadSceneResult)
            // {
            //     var key = config.GetScene(sceneKey).StartRoutKey;
            //
            //     await GoToAsync(key, isFirstRout: true);
            // }

            await HideLoadingScreen();
            
            item.TrySetResult(null);
                
            tasks.Remove(item);

            return loadSceneResult;
        }

        public async UniTask ReloadScene(
            SceneKey sceneKey,
            LoadingScreenKey loadingScreenKey,
            LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            await initTask.Task;
            
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource<RoutContainer>();
            
            tasks.Add(item);

            await UniTask.WhenAll(currentTasks);
            
            await ShowLoadingScreen(loadingScreenKey);

            await scenesModule.ReloadScene(sceneKey, sceneMode);

            await UnloadUnusedAssets();

            await HideLoadingScreen();
        }

        public async UniTask UnloadScene(SceneKey sceneKey, LoadingScreenKey loadingScreenKey)
        {
            await initTask.Task;
            
            var currentTasks = tasks.Select(x => x.Task).ToArray();

            var item = new UniTaskCompletionSource<RoutContainer>();
            
            tasks.Add(item);

            await UniTask.WhenAll(currentTasks);
            
            await ShowLoadingScreen(loadingScreenKey);

            await routerModule.UnloadRouts();

            await scenesModule.UnloadScene(sceneKey);

            await UnloadUnusedAssets();

            await HideLoadingScreen();
            
            item.TrySetResult(null);
                
            tasks.Remove(item);
        }
        
        public async UniTask ShowLoadingScreen(LoadingScreenKey screenKey)
        {
            await loadingScreensModule.Show(screenKey);
        }

        public async UniTask HideLoadingScreen(LoadingScreenKey key = null)
        {
            await loadingScreensModule.Hide(key);
        }
        
        public async UniTask CancelLoadingScreen()
        {            
            await initTask.Task;
            
            await loadingScreensModule.Cancel();
        }
        
        private static async UniTask UnloadUnusedAssets()
        {
            await Observable.Timer(delay);
            await Resources.UnloadUnusedAssets().ToUniTask();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}