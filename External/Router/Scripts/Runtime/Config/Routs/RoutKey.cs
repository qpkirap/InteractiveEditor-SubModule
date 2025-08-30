using System;

namespace Managers.Router.Config
{
    [Serializable]
    public class RoutKey : BaseKey
    {
        public RoutKey()
        {
        }
        
        public RoutKey(string key) : base(key)
        {
        }
        
        public static implicit operator string(RoutKey key) => key.key;
        public static implicit operator RoutKey(string key) => new(key);
    }
}