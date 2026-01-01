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

        public Func<string> getPathValue;
        public Action<string> onPathCreated;

        public Func<Dictionary<string, string>> getDict;

        public string FileName => $"{keyTypeName}s";
    }
}