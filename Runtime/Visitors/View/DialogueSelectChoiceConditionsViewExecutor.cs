using Module.InteractiveEditor.Saves.UI.Story;
using UniRx;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceConditionsViewExecutor : IViewNodeExecute<DialogueSelectChoiceExecutor, DialogueSelectChoiceConditionsCanvas>
    {
        private DialogueSelectChoiceConditionsCanvas uiCanvas;
        private DialogueSelectChoiceExecutor executor;
        
        private readonly CompositeDisposable disp = new();

        public void Inject(DialogueSelectChoiceExecutor execute, DialogueSelectChoiceConditionsCanvas uiCanvas)
        {
            executor = execute;
            this.uiCanvas = uiCanvas;
            
            this.uiCanvas.SetImage(executor.GetBackground());
            this.uiCanvas.SetText(executor.GetText());
            this.uiCanvas.SetCensure(executor.GetCensures());
            this.uiCanvas.SetChoices(executor.GetAnswers(), out var select);
            
            select?.Subscribe(OnSelectChoice).AddTo(disp);
        }
        
        private void OnSelectChoice(int index)
        {
            executor.SetSelectedIndex(index);
        }
        
        public void Reset()
        {
            if (uiCanvas != null) uiCanvas.SetText(new LocalizedString());
            
            disp.Clear();
        }
    }
}