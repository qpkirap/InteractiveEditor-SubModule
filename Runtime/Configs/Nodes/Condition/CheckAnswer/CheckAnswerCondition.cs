using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;
using VContainer;

namespace Module.InteractiveEditor.Configs
{
    public class CheckAnswerCondition : ICondition
    {
        [Inject] private readonly SaveManager saveManager;

        private readonly string idCheckNode;
        private readonly bool isInverse;

        public CheckAnswerCondition(CheckAnswerConditionComponent conditionComponent)
        {
            idCheckNode = conditionComponent.IdNode;
            isInverse = conditionComponent.IsInverse;
        }
        
        public bool IsTrue(BaseNode baseNode)
        {
            var saveItem = saveManager.NodeSaveServices.GetSaveItem(idCheckNode);
            
            if (isInverse)
            {
                return saveItem switch
                {
                    null => false,
                    AnswerChoiceSave saveItemAnswerChoice => !saveItemAnswerChoice.IsSelected,
                    _ => false
                };
            }
            
            return saveItem switch
            {
                null => false,
                AnswerChoiceSave saveItemAnswerChoice => saveItemAnswerChoice.IsSelected,
                _ => false
            };
        }
    }
}