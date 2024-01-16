using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.UI.Story
{
    public class DialogueSelectChoiceCanvas : DialogueCanvas<DialogueSelectChoiceViewExecutor>
    {
        [SerializeField] private ChoiceContainer choiceContainer;
        
        public IObservable<LocalizedString> OnChoice
        {
            get
            {
                return choiceContainer.OnChoice;
            }
        }

        public void SetChoices(IReadOnlyList<LocalizedString> choices)
        {
            if (choiceContainer != null)
            {
                choiceContainer.SetChoices(choices);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            choiceContainer.Disable();
        }
    }
}