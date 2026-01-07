using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer;

namespace Managers.Router.Scene
{
    public class ScenesModule : IDisposable
    {
        [Inject] private readonly IReadOnlyList<IRouterConfig> routerConfigs;

        private readonly Dictionary<SceneKey, AddressableSceneAsset> loadedScenes = new();
        private Dictionary<SceneKey, SceneData> scenesDict;

        private SceneData GetScene(SceneKey key)
        {
            scenesDict ??= routerConfigs
                .Where(c => c != null && c.Scenes != null)
                .SelectMany(c => c.Scenes)
                .Where(x => x != null)
                .ToDictionary(x => (SceneKey)x.Id, x => x);

            if (scenesDict.TryGetValue(key, out var sceneData))
            {
                return sceneData;
            }

            Debug.LogWarning($"Scene with key not found: {key}");
            return null;
        }

        public async UniTask<bool> LoadScene(SceneKey sceneKey, LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            var sceneData = GetScene(sceneKey);

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
