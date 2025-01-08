using Module.InteractiveEditor.Saves.UI.Story;
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

#if UNITY_WEBGL || UNITY_EDITOR
            this.uiCanvas.SetImage(executor.GetBackgroundSprite());
#else
            this.uiCanvas.SetImage(executor.GetBackground());
#endif
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