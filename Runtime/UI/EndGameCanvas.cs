using DepedencyInjection;
using Managers.Router;
using Module.InteractiveEditor.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YG.MenuNav;

namespace Module.InteractiveEditor.UI
{
    public class EndGameCanvas : UICanvas<EndGameViewExecutor>
    {
        [SerializeField] private Button nextButton;
        
        private readonly LazyInject<MenuNavigation> menuNavigation = new();
        
        public Subject<UICanvas> OnNextButtonPressed { get; } = new Subject<UICanvas>();
        
        protected override void OnShow()
        {
            base.OnShow();
            
            if (disp.Count > 0) disp.Clear();

            if (nextButton != null)
                nextButton.OnClickAsObservable().Subscribe(_ => OnNextButtonPressed.OnNext(this)).AddTo(disp);
            
            menuNavigation.Value.SelectButton(nextButton);
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            if (disp.Count > 0) disp.Clear();
        }
    }
}