using Game.UI.Story;

namespace Module.InteractiveEditor.Runtime
{
    public class BaseDialogueViewExecutor : IViewNodeExecute<BaseDialogueExecutor, DialogueCanvas>
    {
        private BaseDialogueExecutor executor;
        private DialogueCanvas uiCanvas;
        
        public void Inject(BaseDialogueExecutor execute, DialogueCanvas uiCanvas)
        {
            executor = execute;
            this.uiCanvas = uiCanvas;
            
            this.uiCanvas.SetImage(executor.GetBackground());
        }

        public void Reset()
        {
        }
    }
}