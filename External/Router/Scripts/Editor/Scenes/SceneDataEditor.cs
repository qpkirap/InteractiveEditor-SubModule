#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Submodules.Router.Editor
{
    [CustomEditor(typeof(SceneData))]
    public class SceneDataEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // Odin Inspector handles the property drawing automatically
            // The titleProperty is now handled by Odin's attribute system
            base.OnInspectorGUI();
        }
    }
}

#endif