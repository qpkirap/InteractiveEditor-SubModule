using Game.UI.Story;
using UniRx;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class ActorDialogueViewExecutor : IViewNodeExecute<ActorDialogueExecutor, ActorDialogueCanvas>
    {
        private readonly CompositeDisposable disp = new();
        
        private ActorDialogueExecutor executor;
        private ActorDialogueCanvas uiCanvas;
        
        public void Inject(ActorDialogueExecutor execute, ActorDialogueCanvas uiCanvas)
        {
            executor = execute;
            this.uiCanvas = uiCanvas;
            
            this.uiCanvas.SetImage(executor.GetBackground());
            this.uiCanvas.SetText(executor.GetText());
            this.uiCanvas.SetActor(executor.GetActor());
            this.uiCanvas.SetCensure(executor.GetCensure());
            
            this.uiCanvas.OnNextButtonPressed.Subscribe(_ => executor.Complete()).AddTo(disp);
        }
        
        public void Reset()
        {
            if (uiCanvas != null)
            {
                uiCanvas.SetText(new LocalizedString());
                uiCanvas.SetActor(null);
            }
            
            disp.Clear();
        }
    }
}