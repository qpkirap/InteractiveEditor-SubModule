using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer;

namespace Managers.Router.Scene
{
    public class ScenesModule : IDisposable
    {
        [Inject] private readonly RouterConfig config;

        private readonly Dictionary<SceneKey, AddressableSceneAsset> loadedScenes = new();

        public async UniTask<bool> LoadScene(SceneKey sceneKey, LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            var sceneData = config.GetScene(sceneKey);

            if (sceneData == null || loadedScenes.ContainsKey(sceneKey))
            {
                return false;
            }

            var sceneAsset = sceneData.Asset;

            sceneAsset.SetLoadSceneMode(sceneMode);

            var sceneInstance = await sceneAsset.LoadAsync();

            loadedScenes.Add(sceneKey, sceneAsset);

            SceneManager.SetActiveScene(sceneInstance.Scene);

            return true;
        }

        public async UniTask UnloadScene(SceneKey sceneKey)
        {
            if (!loadedScenes.TryGetValue(sceneKey, out var sceneAsset))
            {
                return;
            }

            await sceneAsset.ReleaseAsync();

            loadedScenes.Remove(sceneKey);
        }

        public async UniTask ReloadScene(SceneKey sceneKey, LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            await UnloadScene(sceneKey);

            await LoadScene(sceneKey, sceneMode);
        }

        public void Dispose()
        {
            foreach (var sceneAsset in loadedScenes.Values)
            {
                sceneAsset.Dispose();
            }

            loadedScenes.Clear();
        }
    }
}