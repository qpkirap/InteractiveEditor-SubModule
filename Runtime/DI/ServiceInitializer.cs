using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.DI;
using UnityEngine;
using VContainer;

namespace Module.InteractiveEditor.Runtime.DI
{
    /// <summary>
    /// Инициализатор сервисов.
    /// Порядок: Configs → States → Managers → Other → PostInit
    /// Внутри каждой фазы сортируется по InitOrder.
    /// </summary>
    public class ServiceInitializer
    {
        [Inject] private readonly IObjectResolver resolver;
        
        private List<IAsyncConfigInitializable> configInitializables;
        private List<IAsyncStateInitializable> stateInitializables;
        private List<IAsyncManagerInitializable> managerInitializables;
        private List<IAsyncInitializable> otherInitializables;
        private List<IAsyncPostInitializable> postInitializables;
        
        private readonly HashSet<IAsyncInitializable> initializedServices = new();
        private readonly HashSet<IAsyncPostInitializable> postInitializedServices = new();
        
        private void EnsureInitialized()
        {
            if (configInitializables != null) return;
            
            // Phase 1: Configs
            configInitializables = ResolveAndSort<IAsyncConfigInitializable>(resolver);
            
            // Phase 2: States (ISavable)
            stateInitializables = ResolveAndSort<IAsyncStateInitializable>(resolver);
            
            // Phase 3: Managers
            managerInitializables = ResolveAndSort<IAsyncManagerInitializable>(resolver);
            
            // Phase 4: Other (IAsyncInitializable без специфичного интерфейса)
            var allInitializables = ResolveAndSort<IAsyncInitializable>(resolver);
            var specificServices = new HashSet<IAsyncInitializable>();
            specificServices.UnionWith(configInitializables);
            specificServices.UnionWith(stateInitializables);
            specificServices.UnionWith(managerInitializables);
            otherInitializables = allInitializables.Where(x => !specificServices.Contains(x)).ToList();
            
            // Post-init
            postInitializables = ResolveAndSort<IAsyncPostInitializable>(resolver, x => x.PostInitOrder);
        }
        
        private static List<T> ResolveAndSort<T>(IObjectResolver resolver) where T : IAsyncInitializable
        {
            var list = new List<T>();
            try { list.AddRange(resolver.Resolve<IEnumerable<T>>()); } catch { }
            return list.OrderBy(x => x.InitOrder).ToList();
        }
        
        private static List<T> ResolveAndSort<T>(IObjectResolver resolver, Func<T, int> orderSelector)
        {
            var list = new List<T>();
            try { list.AddRange(resolver.Resolve<IEnumerable<T>>()); } catch { }
            return list.OrderBy(orderSelector).ToList();
        }

        /// <summary>
        /// Инициализация: Configs → States → Managers → Other
        /// </summary>
        public async UniTask<bool> Init()
        {
            EnsureInitialized();
            // Phase 1: Configs
            Debug.Log($"[ServiceInitializer] === Phase 1: Configs ({configInitializables.Count}) ===");
            if (!await InitPhase(configInitializables)) return false;
            
            // Phase 2: States
            Debug.Log($"[ServiceInitializer] === Phase 2: States ({stateInitializables.Count}) ===");
            if (!await InitPhase(stateInitializables)) return false;
            
            // Phase 3: Managers
            Debug.Log($"[ServiceInitializer] === Phase 3: Managers ({managerInitializables.Count}) ===");
            if (!await InitPhase(managerInitializables)) return false;
            
            // Phase 4: Other
            if (otherInitializables.Count > 0)
            {
                Debug.Log($"[ServiceInitializer] === Phase 4: Other ({otherInitializables.Count}) ===");
                if (!await InitPhase(otherInitializables)) return false;
            }
            
            return true;
        }
        
        private async UniTask<bool> InitPhase<T>(List<T> services) where T : IAsyncInitializable
        {
            foreach (var service in services)
            {
                if (initializedServices.Contains(service)) continue;
                
                try
                {
                    Debug.Log($"[ServiceInitializer] Init [{service.InitOrder}]: {service.GetType().Name}");
                    await service.Init();
                    initializedServices.Add(service);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ServiceInitializer] Ошибка инициализации {service.GetType().Name}: {ex}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Пост-инициализация.
        /// </summary>
        public async UniTask<bool> PostInit()
        {
            Debug.Log($"[ServiceInitializer] === PostInit ({postInitializables.Count}) ===");
            
            foreach (var service in postInitializables)
            {
                if (postInitializedServices.Contains(service)) continue;
                
                try
                {
                    Debug.Log($"[ServiceInitializer] PostInit [{service.PostInitOrder}]: {service.GetType().Name}");
                    await service.PostInit();
                    postInitializedServices.Add(service);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ServiceInitializer] Ошибка пост-инициализации {service.GetType().Name}: {ex}");
                    return false;
                }
            }
            return true;
        }
    }
}
