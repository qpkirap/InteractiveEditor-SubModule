#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor
{
    [CustomEditor(typeof(RoutData))]
    public class RoutDataEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // Let Odin Inspector handle the properties and embedded list automatically
            base.OnInspectorGUI();
        }
    }
}

#endif