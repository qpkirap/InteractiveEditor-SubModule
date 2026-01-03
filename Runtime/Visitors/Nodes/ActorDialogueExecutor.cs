﻿﻿using System.Collections.Generic;
using System.Linq;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves.UI.Story;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using VContainer;

namespace Module.InteractiveEditor.Runtime
{
    public class ActorDialogueExecutor : INodeExecutor<ActorDialogueNode, ActorDialogueCanvas>
    {
        [Inject] private readonly IRouter router;
        
        private readonly CompositeDisposable disp = new();

        private ImageData imageDataCache;
        private ActorDialogueNode node;
        private ActorDialogueCanvas uiCanvas;
        
        private bool isOpenCanvas;
        private bool isNext;

#if !UNITY_WEBGL

        private AddressableSprite background;

        public AddressableSprite GetBackground()
        {
            var data = GetImageData();
            
            if (data == null) return null;
            
            background ??= data.Image;
            
            return background;
        }
#endif
        
        public Sprite GetBackgroundSprite()
        {
            var data = GetImageData();
            
            if (data == null) return null;

            return data.ImageSprite;
        }

        public IReadOnlyList<CensureData> GetCensure()
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

        public Actor GetActor()
        {
            return node.Actor;
        }
        
        public void Complete()
        {
            isNext = true;
        }
        
        public BaseNode GetNext(ActorDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(ActorDialogueNode baseNode)
        {
            node ??= baseNode;
            
            if (!isOpenCanvas)
            {
                router.GoTo(RoutKeys.actorDialogueCanvas, routArgs: new (string, object)[]
                {
                    (INodeExecutor.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }
            
            return isNext ? ExecuteResult.SuccessState : ExecuteResult.RunningState;
        }

        public ExecuteResult Cancel(ActorDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(ActorDialogueNode baseNode)
        {
            node = null;
            isNext = false;
            isOpenCanvas = false;
            
            imageDataCache = null;
            
#if !UNITY_WEBGL
            background = null;
#endif
        }
        
        // View execution methods
        public void InitializeView(ActorDialogueCanvas canvas)
        {
            uiCanvas = canvas;
            
            if (node != null)
            {
                uiCanvas.SetImage(GetBackgroundSprite());
                uiCanvas.SetText(GetText());
                uiCanvas.SetActor(GetActor());
                uiCanvas.SetCensure(GetCensure());
                
                uiCanvas.OnNextButtonPressed.Subscribe(_ => Complete()).AddTo(disp);
            }
        }

        public void ResetView()
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