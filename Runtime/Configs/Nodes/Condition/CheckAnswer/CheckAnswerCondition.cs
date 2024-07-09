using Module.InteractiveEditor.Runtime;

namespace Module.InteractiveEditor.Configs
{
    public class CheckAnswerCondition : ICondition
    {
        public bool IsTrue(CheckConditionsNode baseNode)
        {
            return true;
        }
    }
}