using System;

namespace Managers.Router.Config
{
    [Serializable]
    public class RoutArgKey : BaseKey
    {
        public RoutArgKey()
        {
        }
        
        public RoutArgKey(string key) : base(key)
        {
        }
        
        public static bool operator ==(RoutArgKey a, RoutArgKey b) => (BaseKey)a == b;
        public static bool operator !=(RoutArgKey a, RoutArgKey b) => (BaseKey)a != b;
        
        public static implicit operator string(RoutArgKey key) => key.key;
        public static implicit operator RoutArgKey(string key) => new(key);
    }
}