using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class CensureViewItem : Button
    {
        public CensureData CensureData { get; private set; }
        
        private bool IsInverseX { get; set; }
        private bool IsInverseY { get; set; }
        
        private float Width { get; set; }
        private float Height { get; set; }
        private Vector2 Center { get; set; }
        
        public CensureViewItem()
        {
            AddToClassList("censure-item");
        }

        public void InjectData(CensureData censureData)
        {
            this.CensureData = censureData;
        }

        public void UpdateData()
        {
            var left = style.left.value.value;
            var top = style.top.value.value;
            var width = style.width.value.value;
            var height = style.height.value.value;
            
            CensureData.SetFieldValue(Configs.CensureData.PositionKey, new Vector2(left, top));
            CensureData.SetFieldValue(Configs.CensureData.SizeKey, new Vector2(width, height));
        }
    }
}