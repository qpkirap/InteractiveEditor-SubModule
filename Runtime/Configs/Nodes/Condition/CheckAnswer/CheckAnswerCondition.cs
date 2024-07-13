using DepedencyInjection;
using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Configs
{
    public class CheckAnswerCondition : ICondition
    {
        private static LazyInject<SaveManager> saveManager = new();

        private readonly string idCheckNode;

        public CheckAnswerCondition(CheckAnswerConditionComponent conditionComponent)
        {
            idCheckNode = conditionComponent.IdNode;
        }
        
        public bool IsTrue(BaseNode baseNode)
        {
            var saveItem = saveManager.Value.NodeSaveServices.GetSaveItem(idCheckNode);

            return saveItem switch
            {
                null => false,
                AnswerChoiceSave saveItemAnswerChoice => saveItemAnswerChoice.IsSelected,
                _ => false
            };
        }
    }
}