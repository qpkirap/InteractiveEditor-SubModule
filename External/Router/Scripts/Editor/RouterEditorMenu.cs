#if UNITY_EDITOR

using Submodules.Router.Editor.Windows;
using UnityEditor;

namespace Submodules.Router.Editor
{
    /// <summary>
    /// Centralized menu for accessing router editor windows
    /// </summary>
    public static class RouterEditorMenu
    {
        [MenuItem("Router/Router Config Editor", priority = 10)]
        public static void OpenRouterConfigEditor()
        {
            RouterConfigWindow.ShowWindow();
        }
    }
}

#endif