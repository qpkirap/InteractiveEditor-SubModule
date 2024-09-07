using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class DialogueSelectChoiceConditionsExecutor : INodeExecute<SelectChoiceDialogueConditionsNode>
    {
        private static LazyInject<IRouter> router = new();
        
        private readonly Dictionary<int, (IEnumerable<ICondition> conditions, BaseNode node)> answersConditions = new();
        private readonly List<LocalizedString> answersCache = new();
        
        private AddressableSprite background;
        private ImageData imageDataCache;
        private SelectChoiceDialogueConditionsNode node;
        
        private bool isOpenCanvas;
        
        public int SelectedIndex { get; private set; }= -1;
        
        public void SetSelectedIndex(int index)
        {
            SelectedIndex = index;
        }

        public IReadOnlyList<LocalizedString> GetAnswers()
        {
            if (answersCache is { Count: > 0 }) return answersCache;
            
            if (node == null) return null;

            var answerChoice = node.ChildrenNodes.Cast<AnswerChoiceConditionsDialogueNode>().ToArray();
            answersConditions.Clear();
            
            for (var i = 0; i < answerChoice.Length; i++)
            {
                var conditions = answerChoice[i].Conditions.Select(x => x.GetCondition());

                answersConditions.Add(i, (conditions, answerChoice[i]));
            }
            
            foreach (var (index, (conditions, node)) in answersConditions)
            {
                var test = true;
                
                foreach (var condition in conditions)
                {
                    if (!condition.IsTrue(node))
                    {
                        test = false;
                        break;
                    }
                }
                
                if (!test) continue;
                
                var answer = answerChoice[index].AnswerText;
                answersCache.Add(answer);
            }

            return answersCache;
        }
        
        public AddressableSprite GetBackground()
        {
            var data = GetImageData();

            if (data == null) return null;
            
            background ??= data.Image;
            
            return background;
        }
        
        private ImageData GetImageData()
        {
            imageDataCache ??= node.RandomImage;

            return imageDataCache;
        }
        
        public IReadOnlyList<CensureData> GetCensures()
        {
            return GetImageData()?.Censures;
        }

        public LocalizedString GetText()
        {
            return node.Dialogue;
        }

        public BaseNode GetNext(SelectChoiceDialogueConditionsNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return null;
            
            if (SelectedIndex >= 0)
            {
                if (answersConditions.TryGetValue(SelectedIndex, out var infos))
                {
                    return infos.node;
                }
            }

            return null;
        }

        public ExecuteResult Execute(SelectChoiceDialogueConditionsNode baseNode)
        {
            node ??= baseNode;
            
            if (baseNode.ChildrenNodes == null || baseNode.ChildrenNodes.Count == 0) return ExecuteResult.SuccessState;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.dialogueSelectChoiceConditions, routArgs: new (string, object)[]
                {
                    (INodeExecute.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }

            var state = SelectedIndex < 0 ? ExecuteResult.RunningState : ExecuteResult.SuccessState;
            
            return state;
        }

        public ExecuteResult Cancel(SelectChoiceDialogueConditionsNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SelectChoiceDialogueConditionsNode baseNode)
        {
            node = null;
            answersCache?.Clear();
            answersConditions?.Clear();
            
            SelectedIndex = -1;
            isOpenCanvas = false;
            
            background = null;
            imageDataCache = null;
        }
    }
}