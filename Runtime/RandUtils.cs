using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public static class RandUtils
    {
        public static T RandomItem<T>(this IEnumerable<T> items)
        {
            return items.ElementAtOrDefault(Random.Range(0, items.Count()));
        }
    }
}