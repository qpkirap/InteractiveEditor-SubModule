using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using Managers.Router.Routs;
using UniRx;
using UnityEngine.SceneManagement;

namespace Managers.Router
{
    public interface IRouter
    {
        IReadOnlyReactiveProperty<RoutContainer> CurrentRout { get; }
        IReadOnlyCollection<RoutContainer> CurrentRouts { get; }
        
        UniTask GoTo(RoutKey routKey,
            LoadingScreenKey loadingScreenKey = null,
            Func<bool> isActivateLoading = null,
            bool isFirstRout = false,
            bool useAnimation = true,
            params (string argKey, object data)[] routArgs);

        UniTask GoBack(bool ignoreBackPress = false, bool useAnimation = true);
        
        UniTask<bool> LoadScene(SceneKey sceneKey, LoadingScreenKey loadingScreenKey = null, LoadSceneMode sceneMode = LoadSceneMode.Additive);
        UniTask UnloadScene(SceneKey sceneKey, LoadingScreenKey loadingScreenKey = null);
        
        UniTask ShowLoadingScreen(LoadingScreenKey screenKey);
        UniTask HideLoadingScreen(LoadingScreenKey screenKey = null);   

        T GetRoutArgData<T>(string argKey);

        void SetUserControlState(bool state);
    }
}