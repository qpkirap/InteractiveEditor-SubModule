#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Submodules.Router.Editor
{
    [CustomEditor(typeof(RouterConfig))]
    public class RouterConfigEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // Odin Inspector automatically handles the lists with proper attributes
            // The scenes, routs, and loadings lists will be rendered by Odin
            // with built-in add/remove functionality and better UI
            base.OnInspectorGUI();
        }
    }
#endif
}