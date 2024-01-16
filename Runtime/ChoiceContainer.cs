using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Pool;

namespace Module.InteractiveEditor.Runtime
{
    public class ChoiceContainer : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private ChoiceItem prefab;

        private IObjectPool<ChoiceItem> pool;
        private readonly List<ChoiceItem> activeItems = new();

        public async UniTask Init()
        {
            pool ??= new ObjectPool<ChoiceItem>(
                () => Instantiate(prefab, container), 
                item =>
                {
                    activeItems.Add(item);
                    item.gameObject.SetActive(true);
                },
                item =>
                {
                    item.Disable();
                    item.gameObject.SetActive(false);
                    activeItems.Remove(item);
                });
        }

        public void Disable()
        {
            foreach (var item in activeItems)
            {
                pool.Release(item);
            }
            
            activeItems.Clear();
        }

        public void SetChoices(IReadOnlyList<LocalizedString> choices)
        {
            if (choices is { Count: 0 }) return;

            foreach (var text in choices)
            {
                var item = pool.Get();
                
                item.SetText(text);
            }
        }
    }
}