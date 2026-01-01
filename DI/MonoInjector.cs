using UnityEngine;

namespace DepedencyInjection
{
    public class MonoInjector : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] behaviours;
        [SerializeField] private ScriptableObject[] scriptables;
        [SerializeField] private string injectTag;

        private void Awake() => DInjector.OnRootCreated += OnRootCreated;
        private void OnDestroy() => DInjector.OnRootCreated -= OnRootCreated;

        public string InjectTag
        {
            get => injectTag;
            set => injectTag = value;
        }

        private void OnRootCreated(CompositionRoot root)
        {
            if (!string.Equals(injectTag, root.Tag, System.StringComparison.InvariantCulture)) return;

            for (int i = 0; i < behaviours.Length; ++i)
            {
                var t = behaviours[i].GetType();
                root.Add(t, behaviours[i]);
            }

            for (int i = 0; i < scriptables.Length; ++i)
            {
                var t = scriptables[i].GetType();
                root.Add(t, scriptables[i]);
            }
        }
    }
}