using Managers.Router.Config.Loading;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Submodules.Router.Editor
{
    [CustomEditor(typeof(LoadingScreenData))]
    public class LoadingDataEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // Odin Inspector handles the property drawing automatically
            // The titleProperty is now handled by Odin's attribute system
            base.OnInspectorGUI();
        }
    }
}