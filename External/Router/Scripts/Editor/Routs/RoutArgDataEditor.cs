using Managers.Router.Config;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

#if UNITY_EDITOR

namespace Submodules.Router.Editor
{
    [CustomEditor(typeof(RoutArgData))]
    public class RoutArgDataEditor : OdinEditor
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