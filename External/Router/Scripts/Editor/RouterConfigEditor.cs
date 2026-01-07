#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Submodules.Router.Editor
{
    [CustomEditor(typeof(BaseRouterConfig))]
    public class RouterConfigEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}