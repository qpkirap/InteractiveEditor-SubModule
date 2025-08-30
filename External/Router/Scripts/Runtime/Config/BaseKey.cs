using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Managers.Router.Config
{
    public class BaseKey : ICloneable, IEquatable<BaseKey>
    {
        [JsonProperty, SerializeField] protected string key = "";

        public BaseKey()
        {
        }

        public BaseKey(string key)
        {
            this.key = key;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool Contains(string value)
        {
            return key.Contains(value);
        }

        public override string ToString()
        {
            return key;
        }

        #region IEquatable

        public bool Equals(BaseKey other)
        {
            return !string.IsNullOrEmpty(key)
                   && other is not null
                   && !string.IsNullOrEmpty(other.key)
                   && key == other.key;
        }

        public override bool Equals(object other)
        {
            return other is BaseKey baseKey
                   && Equals(baseKey);
        }

        public override int GetHashCode()
        {
            return key?.GetHashCode() ?? 0;
        }

        public static bool operator ==(BaseKey a, BaseKey b)
        {
            return a is null || b is null
                ? Equals(a, b)
                : a.Equals(b);
        }

        public static bool operator !=(BaseKey a, BaseKey b)
        {
            return a is null || b is null
                ? !Equals(a, b)
                : !a.Equals(b);
        }

        #endregion
    }
}