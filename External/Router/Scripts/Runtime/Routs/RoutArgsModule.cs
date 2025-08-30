using System;
using System.Collections.Generic;
using System.Linq;
using Managers.Router.Config;

namespace Managers.Router.Routs
{
    public class RoutArgsModule : IDisposable
    {
        private readonly Dictionary<string, object> routArgsDict = new();
        private readonly Dictionary<RoutKey, string[]> routArgKeysDict = new();

        public T GetRoutArgData<T>(string argKey)
        {
            return routArgsDict.TryGetValue(argKey, out var data)
                ? (T)data
                : default;
        }

        public void AddArgData(RoutKey routKey, params (string argKey, object data)[] routArgs)
        {
            RemoveArgData(routKey);

            foreach (var (argKey, data) in routArgs)
            {
                routArgsDict[argKey] = data;
            }

            routArgKeysDict[routKey] = routArgs
                .Select(x => x.argKey)
                .ToArray();
        }

        public void RemoveArgData(RoutKey routKey)
        {
            if (routArgKeysDict.TryGetValue(routKey, out var routArgKeys))
            {
                foreach (var argKey in routArgKeys)
                {
                    routArgsDict.Remove(argKey);
                }

                routArgKeysDict.Remove(routKey);
            }
        }

        public void Dispose()
        {
            routArgsDict.Clear();
            routArgKeysDict.Clear();
        }
    }
}