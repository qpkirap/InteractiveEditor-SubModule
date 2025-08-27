﻿﻿﻿using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves.UI.Story;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class BaseDialogueExecutor : INodeExecutor<BaseDialogueNode, DialogueCanvas>
    {
        private static LazyInject<IRouter> router = new();
        private readonly CompositeDisposable disp = new();

        private ImageData imageDataCache;
        private BaseDialogueNode node;
        private AddressableSprite background;
        
        private bool isOpenCanvas;
        private bool isNext;
        
        public ExecuteResult Execute(BaseDialogueNode baseNode)
        {
            node ??= baseNode;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.baseDialogue, routArgs: new (string, object)[]
                {
                    (INodeExecutor.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }
            
            return isNext ? ExecuteResult.SuccessState : ExecuteResult.RunningState;
        }

        public ExecuteResult Cancel(BaseDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(BaseDialogueNode baseNode)
        {
            node = null;
            isNext = false;
            isOpenCanvas = false;
            
            background = null;
            imageDataCache = null;
            disp.Clear();
        }

#if !UNITY_WEBGL
         public AddressableSprite GetBackground()
        {
            var data = GetImageData();

            if (data == null) return null;
            
            background ??= data.Image;
            
            return background;
        }
#endif
       
        public Sprite GetSprite()
        {
            return GetImageData()?.ImageSprite;
        }
        
        public IReadOnlyList<CensureData> GetCensures()
        {
            return GetImageData()?.Censures;
        }
        
        private ImageData GetImageData()
        {
            imageDataCache ??= node.GetRandomData;

            return imageDataCache;
        }
        
        public LocalizedString GetText()
        {
            return node.Dialogue;
        }

        public void Complete()
        {
            isNext = true;
        }

        public BaseNode GetNext(BaseDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }
        
        // View execution methods
        public void InitializeView(DialogueCanvas uiCanvas)
        {
            if (uiCanvas == null) return;
            
            // Set up the dialogue UI with current node data
            UpdateDialogueUI(uiCanvas);
            
            // Subscribe to next button if available
            if (uiCanvas.OnNextButtonPressed != null)
            {
                uiCanvas.OnNextButtonPressed.Subscribe(_ => Complete()).AddTo(disp);
            }
        }
        
        public void ResetView()
        {
            // Reset any view-specific state if needed
            // The canvas will handle its own cleanup in OnHide
            disp.Clear();
        }
        
        private void UpdateDialogueUI(DialogueCanvas uiCanvas)
        {
            if (node == null) return;
            
            // Set text
            uiCanvas.SetText(GetText());
            
            // Set background image
#if !UNITY_WEBGL
            var background = GetBackground();
            if (background != null)
            {
                uiCanvas.SetImage(background);
            }
            else
#endif
            {
                var sprite = GetSprite();
                if (sprite != null)
                {
                    uiCanvas.SetImage(sprite);
                }
            }
            
            // Set censures
            var censures = GetCensures();
            if (censures != null)
            {
                uiCanvas.SetCensure(censures);
            }
        }
    }
}