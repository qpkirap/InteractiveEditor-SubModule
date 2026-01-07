using System;
using System.Collections.Generic;

namespace Module.Utils.Editor
{
    public class KeyScriptData
    {
        public string keyTypeName;
        public string namespaceValue;

        public string defaultKey;
        public string defaultValue;
        
        /// <summary>
        /// Кастомное имя файла (без расширения). Если не задано, используется keyTypeName + "s"
        /// </summary>
        public string customFileName;

        public Func<string> getPathValue;
        public Action<string> onPathCreated;

        public Func<Dictionary<string, string>> getDict;

        public string FileName => string.IsNullOrEmpty(customFileName) ? $"{keyTypeName}s" : customFileName;
    }
}