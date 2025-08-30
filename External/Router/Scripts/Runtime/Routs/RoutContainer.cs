using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.Router.Config;
using UniRx;
using UnityEngine;

namespace Managers.Router.Routs
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public abstract class RoutContainer : MonoBehaviour
    {
        private Canvas canvas;
        private CanvasGroup canvasGroup;

        private RoutData data;
        
        private readonly List<UniTaskCompletionSource> tasks = new();

        private long deactiveTime;

        public RoutData Data => data;

        internal long DeactiveTime => deactiveTime;
        internal bool IsDeactivate => deactiveTime != -1;

        internal Canvas Canvas => canvas ??= GetComponent<Canvas>();
        internal CanvasGroup CanvasGroup => canvasGroup ??= GetComponent<CanvasGroup>();
        
        internal Subject<RoutContainer> OnDispose { get; } = new();
        
        public bool IsReady => tasks.Count == 0;

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>        
        /// Инициализация выполняется один раз при создании контейнера,
        /// Будьте внимательны при выполнении подписок на события и загрузке ресурсов!
        /// </summary>
        public virtual async UniTask Init()
        {
        }

        /// <summary>
        /// Выполняется один раз после создания контейнера, после Init(), после активации экрана
        /// </summary>
        public virtual async UniTask PostInit()
        {
        }

        internal void SetRoutData(RoutData data)
        {
            this.data = data;
        }

        internal async UniTask Show(bool useAnimation = true)
        {
            if (CanvasGroup == null)
            {
                Debug.LogError($"CanvasGroup is Null {gameObject.name}");
                
                return;
            }
            
            CanvasGroup.alpha = 1;
            CanvasGroup.blocksRaycasts = true;
            
            OnShow();
            
            gameObject.SetActive(true);

            if (useAnimation)
            {
                var item = new UniTaskCompletionSource();
                
                tasks.Add(item);

                ShowAnimation().ContinueWith(() =>
                {
                    item.TrySetResult();

                    return tasks.Remove(item);
                });
            }
        }

        /// <summary>
        /// Можно исользовать для анимаций срабатывает если при показе экрана указано с анимацей
        /// </summary>
        protected virtual async UniTask ShowAnimation()
        {
        }

        /// <summary>
        /// Выполняется при показе (фокусе) экрана перед gameObject.SetActive(true)
        /// </summary>
        protected virtual void OnShow()
        {
        }

        internal async UniTask Hide(bool useAnimation = true)
        {
            if (CanvasGroup == null)
            {
                Debug.LogError($"CanvasGroup is Null {gameObject.name}");
                
                return;
            }
            
            var currentTasks = tasks.Select(x => x.Task).ToArray();
            
            await UniTask.WhenAll(currentTasks);
            
            CanvasGroup.alpha = 0;
            CanvasGroup.blocksRaycasts = false;

            if (useAnimation)
            {
                var item = new UniTaskCompletionSource();
                
                tasks.Add(item);

                await HideAnimation().ContinueWith(() =>
                {
                    item.TrySetResult();

                    return tasks.Remove(item);
                });
            }
            
            gameObject.SetActive(false);
            
            OnHide();
            
            SetLoadStatus(false);
        }

        /// <summary>
        /// Выполняется после закрытия экрана (gameObject.SetActive(false))
        /// </summary>
        protected virtual void OnHide()
        {
        }
        
        protected virtual async UniTask HideAnimation()
        {
        }

        internal void SetLoadStatus(bool state)
        {
            if (state)
            {
                deactiveTime = -1;
            }
            else
            {
                deactiveTime = (long)Time.time;
            }
        }

        internal void SetSortOrder(int sortOrder)
        {
            Canvas.sortingOrder = sortOrder;
        }

        /// <summary>
        /// Проверка возможности закрыть контейнер (выполнить GoBack)
        /// </summary>
        /// <returns>
        /// <c>true</c> - закрыть контейнер<br/>
        /// <c>false</c> - если нужно выполнить свою логику перед закрытием контейнера
        /// </returns>
        internal bool OnBack(bool ignoreBackPress = false)
        {
            return ignoreBackPress || IsAvailableBack(ignoreBackPress);
        }

        /// <summary>
        /// Может ли роутер выполнить GoBack (перейти на предыдущий экран)
        /// </summary>
        /// <param name="ignoreBackPress"></param>
        /// <returns></returns>
        protected virtual bool IsAvailableBack(bool ignoreBackPress)
        {
            return true;
        }
        
        internal bool IsAvailableExit()
        {
            return IsAvailableExitAlert();
        }
        
        /// <summary>
        /// Возможность показать алерт выхода из игры (если не показано других корных алертов)
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsAvailableExitAlert()
        {
            return false;
        }

        public virtual void Dispose()
        {
            OnDispose.OnNext(this);
        }
    }
}