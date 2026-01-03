using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Managers.Router.Routs
{
    public class RouterModule
    {
        private const string controllerName = "--- Router Controller ---";
        
        [Inject] private readonly RouterConfig config;
        [Inject] private readonly IObjectResolver resolver;
        
        private readonly LinkedList<RoutContainer> routs = new();
        private readonly LinkedList<RoutKey> routKeysHistory = new();
        
        private RouterController controller;
        private AddressableGameObject controllerAsset;
        
        private RoutContainer CurrentRout => routs.First?.Value;
        public IReadOnlyCollection<RoutContainer> Routs => routs;
        
        public Subject<RoutData> OnStartLoadRout { get; } = new();
        
        public bool IsAvailableBack => routKeysHistory.Count > 1;
        
        public async UniTask Init()
        {
            await CheckController();
        }
        
        public void SetUserControlState(bool state)
        {
            if (controller != null)
            {
                controller.SetUserControlState(state);
            }
        }
        
        public async UniTask<RoutContainer> GoToAsync(RoutKey routKey, bool isFirstRout, bool useAnimation = true)
        {
            if (isFirstRout)
            {
                ClearRouts();
            }
            
            var routData = config.GetRout(routKey);

            var rout = await ShowRoutContainer(routKey, useAnimation);

            // upd prev rout
            var prevRout = CurrentRout;
            var isPrevEqualCurrent = prevRout != null && prevRout.Data.Id == rout.Data.Id;

            if (prevRout != null && routData.HidePrevRout && !isPrevEqualCurrent) await prevRout.Hide();
            
            routs.AddFirst(rout);
            routKeysHistory.AddFirst(routKey);

            if (rout.Data.OverrideSortOrder)
            {
                rout.Canvas.sortingOrder = rout.Data.SortOrder;
            }

            return rout;
        }
        
        private async UniTask<RoutContainer> ShowRoutContainer(RoutKey routKey, bool useAnimation = true)
        {
            if (routKey == null)
            {
                return null;
            }

            await CheckController();
            
            var routData = config.GetRout(routKey);
            var sortOrder = config.GetRoutSortOrder(routData) + routKeysHistory.Count;
            
            OnStartLoadRout.OnNext(routData);
            
            var (rout, isNewLoad) = await controller.GetOrCreateContainer(routKey, routData, sortOrder);
            
            rout.SetLoadStatus(true);
            rout.SetRoutData(routData);
            
            resolver.InjectGameObject(rout.gameObject);

            if (isNewLoad)
            {
                rout.OnDispose.Subscribe(OnDisposeRout).AddTo(rout.gameObject);
                
                await rout.Init();

                await UniTask.DelayFrame(1); //ждем пока сработает скейлеры и прочие
            }
            
            await rout.Show(useAnimation);

            if (isNewLoad) await rout.PostInit();

            return rout;
        }
        
        public async UniTask<RoutContainer> GoBack(bool ignoreBackPress, bool useAnimation = true)
        {
            if (routs.Count == 1)
            {
                Debug.LogError($"routs.Count == 1");
                
                return null;
            }
            
            var prevRout = CurrentRout;
            
            Debug.Log($"StartBack");

            if (prevRout == null)
            {
                return null;
            }

            var backCheck = prevRout.OnBack(ignoreBackPress);

            if (backCheck)
            {
                routs.RemoveFirst();
                routKeysHistory.RemoveFirst();

                var routKey = routKeysHistory.First.Value;
                
                Debug.Log($"current {routKey}");

                if (string.IsNullOrEmpty(routKey)) return null;

                await ShowRoutContainer(routKey);
                
                await prevRout.Hide(useAnimation);
            }

            return routs.First?.Value;
        }
        
        public async UniTask UnloadRouts(bool disposeController = false)
        {
            if (disposeController)
            {
                DisposeController();
            }
            else
            {
                await controller.UnloadRouts();
            }
        }
        
        private async UniTask CheckController()
        {
            if (controller == null)
            {
                controllerAsset = config.RouterControllerAsset;

                var containerGO = await controllerAsset.LoadAsync();

                controller = containerGO.GetComponent<RouterController>();
                controller.name = controllerName;
            }
        }
        
        private void ClearRouts()
        {
            foreach (var rout in routs)
            {
                rout.Hide();
            }

            routs.Clear();
        }
        
        private void DisposeController()
        {
            if (controller != null)
            {
                controller.Dispose();
                controllerAsset.Dispose();

                controller = null;
                controllerAsset = null;
            }
        }
        
        private void OnDisposeRout(RoutContainer rout)
        {
            if (rout == null) return;

            var all = routs.Where(x => x == rout).ToArray();

            foreach (var routContainer in all)
            {
                routs.Remove(routContainer);
            }
        }
    }
}