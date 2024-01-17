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

        public void SetChoices(IReadOnlyList<LocalizedString> choices, out IObservable<int> selected)
        {
            if (choiceContainer != null)
            {
                choiceContainer.SetChoices(choices);

                selected = choiceContainer.OnChoicePressed;
            }
            
            selected = null;
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            choiceContainer.Disable();
        }
    }
}