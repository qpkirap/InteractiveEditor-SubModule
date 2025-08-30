using System;

namespace Managers.Router.Config
{
    [Serializable]
    public class SceneKey : BaseKey
    {
        public SceneKey()
        {
        }
        
        public SceneKey(string key) : base(key)
        {
        }
        
        public static implicit operator string(SceneKey key) => key.key;
        public static implicit operator SceneKey(string key) => new(key);
    }
}