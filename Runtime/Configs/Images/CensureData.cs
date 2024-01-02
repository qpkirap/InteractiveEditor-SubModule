using System;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class CensureData
    {
        [field: SerializeField] private Vector2 position;
        [field: SerializeField] private Vector2 size;

        #region Editor

        public const string PositionKey = nameof(position);
        public const string SizeKey = nameof(size);

        #endregion
        
        public Vector2 Position => position;
        public Vector2 Size => size;

        public CensureData Clone()
        {
            return (CensureData)MemberwiseClone();
        }
    }
}