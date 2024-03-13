using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Linq;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Game.UI.Story
{
    public class DialogueSelectChoiceCanvas : UICanvas<DialogueSelectChoiceViewExecutor>
    {
        [SerializeField] private List<ImageController> bgImages;
        [SerializeField] private CensorContainerController censorController;
        [SerializeField] private TextController textController;
        [SerializeField] private ChoiceContainer choiceContainer;

        protected override void OnShow()
        {
            base.OnShow();
            
            bgImages.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async item =>
            {
                await item.Init();
            });
            
            choiceContainer.Init();
            textController.Init();
        }
        
        public void SetImage(AddressableSprite sprite)
        {
            bgImages.ForEach(item => item.SetImage(sprite));
        }
        
        public void SetText(LocalizedString localizedString)
        {
            textController.SetText(localizedString);
        }

        public void SetCensure(IReadOnlyList<CensureData> censures)
        {
            censorController.InjectData(censures);
        }

        public void SetChoices(IReadOnlyList<LocalizedString> choices, out IObservable<int> selected)
        {
            selected = null;
            
            if (choiceContainer != null)
            {
                choiceContainer.SetChoices(choices);

                selected = choiceContainer.OnChoicePressed;
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            bgImages.ForEach(item => item.Disable());
            
            censorController.Disable();
            textController.Disable();
            
            choiceContainer.Disable();
        }
    }
}