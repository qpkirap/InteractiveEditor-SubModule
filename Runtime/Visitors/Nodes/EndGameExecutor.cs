﻿using System.Linq;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves;
using Module.InteractiveEditor.UI;
using UniRx;
using UnityEngine;
using VContainer;

namespace Module.InteractiveEditor.Runtime
{
    public class EndGameExecutor : INodeExecutor<EndGameNode, EndGameCanvas>
    {
        [Inject] private readonly IRouter router;
        [Inject] private readonly StoryObjectManager storyManager;
        [Inject] private readonly SaveManager saveManager;
        
        private readonly CompositeDisposable disp = new();
        
        private EndGameNode node;
        
        private bool isOpenCanvas;
        private bool isNext;
        
        public BaseNode GetNext(EndGameNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(EndGameNode baseNode)
        {
            node ??= baseNode;
            
            if (!isOpenCanvas)
            {
                router.GoTo(RoutKeys.endGame, routArgs: new (string, object)[]
                {
                    (INodeExecutor.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }
            
            return isNext ? ExecuteResult.SuccessState : ExecuteResult.RunningState;
        }

        public ExecuteResult Cancel(EndGameNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(EndGameNode baseNode)
        {
            node = null;
            isNext = false;
            isOpenCanvas = false;
            disp.Clear();
        }
        
        // View execution methods
        public void InitializeView(EndGameCanvas uiCanvas)
        {
            if (uiCanvas == null) return;
            
            // Subscribe to next button if available
            if (uiCanvas.OnNextButtonPressed != null)
            {
                uiCanvas.OnNextButtonPressed.Subscribe(_ => Complete()).AddTo(disp);
            }
        }
        
        public void ResetView()
        {
            // Reset any view-specific state if needed
            disp.Clear();
        }
        
        public void Complete()
        {
            saveManager.NodeSaveServices.SetLastIdNode(string.Empty);
            saveManager.Reset();
            storyManager.Reload();
            
            isNext = true;
        }
    }
}