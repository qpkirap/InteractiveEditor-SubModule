using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class UIButton : Button
    {
        private readonly Subject onUp = new();
        private readonly Subject onDown = new();
        private new readonly Subject onClick = new();
        
        public static Vector3 ClickPositionLast { get; private set; }

        public IObservable OnUp => onUp;
        public IObservable OnDown => onDown;
        public IObservable OnClick => onClick;

        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            
            this.OnPointerClickAsObservable()
                .Subscribe(_ => OnClickCallback())
                .AddTo(this);

            this.OnPointerDownAsObservable()
                .Subscribe(_ => OnDownCallback())
                .AddTo(this);

            this.OnPointerUpAsObservable()
                .Subscribe(_ => OnUpCallback())
                .AddTo(this);
        }

        public void SetInteractableState(bool state)
        {
            interactable = state;
        }

        private void OnUpCallback()
        {
            if (!interactable)
            {
                return;
            }

            onUp.OnNext();
        }

        private void OnDownCallback()
        {
            if (!interactable)
            {
                return;
            }
            
            onDown.OnNext();
        }

        public void OnClickCallback()
        {
            if (!interactable)
            {
                return;
            }

            onClick.OnNext();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            ClickPositionLast = eventData.position;
        }
    }
}