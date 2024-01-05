using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.Pool;

namespace Module.InteractiveEditor.Runtime
{
    public class CensorContainerController : MonoBehaviour
    {
        [SerializeField] private CensorItem prefab;
        [SerializeField] private RectTransform parentItems;

        private ObjectPool<CensorItem> pool;
        private readonly List<CensorItem> activeItems = new();
        
        public void InjectData(IReadOnlyList<CensureData> data)
        {
            ClearActiveItems();
            
            if (data == null) return;
            
            InitPool();
            
            foreach (var censureData in data)
            {
                if (censureData == null) continue;
                
                pool.Get().InjectData(censureData);
            }
        }

        public void Disable()
        {
            ClearActiveItems();
        }

        private void ClearActiveItems()
        {
            activeItems.ToList().ForEach(item => pool?.Release(item));
        }

        #region Pool

        private void InitPool()
        {
            if (pool != null) return;
            
            pool = new(
                () => Instantiate(prefab, parentItems),
                item =>
                {
                    item.gameObject.SetActive(true);
                    
                    activeItems.Add(item);
                },
                item =>
                {
                    item.gameObject.SetActive(false);

                    activeItems.Remove(item);
                },
                item =>
                {
                    activeItems.Remove(item);
                    
                    Destroy(item.gameObject);
                });
        }

        #endregion
    }
}