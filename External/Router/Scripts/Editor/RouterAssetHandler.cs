#if UNITY_EDITOR

using Managers.Router.Config;
using Managers.Router.Config.Loading;
using Submodules.Router.Editor.Windows;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Submodules.Router.Editor
{
    /// <summary>
    /// Handles double-click opening of router-related assets in dedicated Odin Inspector windows
    /// </summary>
    public static class RouterAssetHandler
    {
        // Double-click handlers for router assets (priority 2 to avoid conflicts with other handlers)
        [OnOpenAsset(2)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            
            // Handle RouterConfig double-click
            if (asset is RouterConfig routerConfig)
            {
                RouterConfigWindow.ShowWindow(routerConfig);
                return true;
            }
            
            // Handle SceneData double-click
            if (asset is SceneData sceneData)
            {
                SceneDataWindow.ShowWindow(sceneData);
                return true;
            }
            
            // Handle RoutData double-click
            if (asset is RoutData routData)
            {
                RoutDataWindow.ShowWindow(routData);
                return true;
            }
            
            // Handle LoadingScreenData double-click
            if (asset is LoadingScreenData loadingScreenData)
            {
                LoadingScreenDataWindow.ShowWindow(loadingScreenData);
                return true;
            }
            
            // Handle RoutArgData double-click
            if (asset is RoutArgData routArgData)
            {
                RoutArgDataWindow.ShowWindow(routArgData);
                return true;
            }
            
            return false; // Let Unity handle other asset types
        }
    }
}

#endif