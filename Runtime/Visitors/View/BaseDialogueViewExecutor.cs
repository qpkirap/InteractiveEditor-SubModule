using Game.UI.Story;
using UniRx;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class BaseDialogueViewExecutor : IViewNodeExecute<BaseDialogueExecutor, DialogueCanvas>
    {
        private readonly CompositeDisposable disp = new();
        
        private BaseDialogueExecutor executor;
        private DialogueCanvas uiCanvas;
        
        public void Inject(BaseDialogueExecutor execute, DialogueCanvas uiCanvas)
        {
            executor = execute;
            this.uiCanvas = uiCanvas;
            
            this.uiCanvas.SetImage(executor.GetBackground());
            this.uiCanvas.SetText(executor.GetText());
            this.uiCanvas.SetCensure(executor.GetCensures());

            this.uiCanvas.OnNextButtonPressed.Subscribe(_ => executor.Complete()).AddTo(disp);
        }

        public void Reset()
        {
            if (uiCanvas != null) uiCanvas.SetText(new LocalizedString());
            
            disp.Clear();
        }
    }
}