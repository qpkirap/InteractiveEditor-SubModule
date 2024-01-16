using Game.UI.Story;
using UniRx;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceViewExecutor : IViewNodeExecute<DialogueSelectChoiceExecutor, DialogueSelectChoiceCanvas>
    {
        private readonly CompositeDisposable disp = new();
        
        private DialogueSelectChoiceExecutor executor;
        private DialogueSelectChoiceCanvas uiCanvas;
        
        public void Inject(DialogueSelectChoiceExecutor execute, DialogueSelectChoiceCanvas uiCanvas)
        {
            executor = execute;
            this.uiCanvas = uiCanvas;
            
            this.uiCanvas.SetImage(executor.GetBackground());
            this.uiCanvas.SetText(executor.GetText());
            this.uiCanvas.SetCensure(executor.GetCensures());
            this.uiCanvas.SetChoices(executor.GetAnswers());
        }
        
        public void Reset()
        {
            if (uiCanvas != null) uiCanvas.SetText(new LocalizedString());
            
            disp.Clear();
        }
    }
}