using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
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
        private readonly CompositeDisposable disp = new();
        private readonly List<ChoiceItem> activeItems = new();
        
        public Subject<int> OnChoicePressed { get; } = new();

        public async UniTask Init()
        {
            ClearItems();
            
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
            ClearItems();
        }

        private void ClearItems()
        {
            disp.Clear();
            
            foreach (var item in activeItems.ToArray())
            {
                pool.Release(item);
            }
            
            activeItems.Clear();
        }

        public void SetChoices(IReadOnlyList<LocalizedString> choices)
        {
            ClearItems();
            
            if (choices is { Count: 0 }) return;

            for (var i = 0; i < choices.Count; i++)
            {
                var text = choices[i];
                var item = pool.Get();

                item.InjectData(i, text);
                
                item.OnClickAsObservable.Subscribe(index => OnChoicePressed.OnNext(index)).AddTo(disp);
            }
        }
    }
}