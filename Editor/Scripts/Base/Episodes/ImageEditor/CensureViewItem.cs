using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class CensureViewItem : Button
    {
        public CensureData CensureData { get; private set; }
        
        public CensureViewItem()
        {
            AddToClassList("censure-item");
        }

        public void InjectData(CensureData censureData)
        {
            this.CensureData = censureData;
            
            text = "censure";
        }

        public void UpdateData()
        {
            var left = style.left.value.value;
            var top = style.top.value.value;
            var width = style.width.value.value;
            var height = style.height.value.value;
            
            CensureData.SetFieldValue(CensureData.PositionKey, new Vector2(left, top));
            CensureData.SetFieldValue(CensureData.SizeKey, new Vector2(width, height));
        }
    }
}