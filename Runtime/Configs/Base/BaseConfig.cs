using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class BaseConfig : ScriptableObject
    {
        #region editor
        /// <summary> 
        /// Ссылка на папку конфига
        /// </summary>
        /// <remarks>
        /// Используется для формирования пути сохранения @UnityEngine.ScriptableObject ресурсов конфига
        /// </remarks>
        [SerializeField] protected Object configFolder = null;

        /// <summary>
        /// Проверка ссылки на папку конфига
        /// </summary>
        public bool HasConfigFolder()
        {
            return configFolder != null;
        }
        #endregion
    }
}