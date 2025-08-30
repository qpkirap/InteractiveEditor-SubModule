using System;

namespace Managers.Router.Config
{
    [Serializable]
    public class LoadingScreenKey : BaseKey
    {
        public LoadingScreenKey() : base()
        {
        }
        
        public LoadingScreenKey(string key) : base(key)
        {
        }
        
        public static implicit operator string(LoadingScreenKey key) => key.key;
        public static implicit operator LoadingScreenKey(string key) => new(key);
    }
}