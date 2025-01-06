using Module.InteractiveEditor.UI;
using UniRx;

namespace Module.InteractiveEditor.Runtime
{
    public class EndGameViewExecutor : IViewNodeExecute<EndGameExecutor, EndGameCanvas>
    {
        private readonly CompositeDisposable disp = new();
        
        private EndGameExecutor executor;
        private EndGameCanvas uiCanvas;
        
        public void Reset()
        {
            disp.Clear();
        }

        public void Inject(EndGameExecutor execute, EndGameCanvas uiCanvas)
        {
            executor = execute;
            this.uiCanvas = uiCanvas;
            
            this.uiCanvas.OnNextButtonPressed.Subscribe(_ => executor.Complete()).AddTo(disp);

        }
    }
}