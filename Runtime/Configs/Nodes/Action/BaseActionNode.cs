using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class BaseActionNode : BaseNode<ActionExecutor>
    {
        [SerializeField] [HideInInspector] private List<ActionTaskComponent> tasks;

        private IUniTaskAsyncEnumerable<ActionTaskComponent> collection;

        public IReadOnlyList<ActionTaskComponent> Tasks => tasks;

        #region Editor

        public const string TasksKey = nameof(tasks);

        #endregion
        

        public override object Clone()
        {
            var item = (BaseNode)base.Clone();
            
            item.RemoveCloneSuffix();

            var taskCloneList = new List<ActionTaskComponent>();
            
            foreach (var task in this.tasks)
            {
                if (task == null) continue;
                
                var taskClone = task.Clone() as ActionTaskComponent;
                
                taskClone.RemoveCloneSuffix();
                taskClone.SetId(task.Id);
                
                taskCloneList.Add(taskClone);
            }

            item.SetFieldValue(TasksKey, taskCloneList);

            return item;
        }
    }
}
