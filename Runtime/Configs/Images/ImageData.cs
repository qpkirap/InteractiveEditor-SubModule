using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Component = Module.InteractiveEditor.Runtime.Component;

namespace Module.InteractiveEditor.Configs
{
    public class ImageData : Component
    {
        [SerializeField] private AssetReference image;
        [SerializeField] private List<Vector2> censures;

        private AddressableSprite imageCache;

        public AddressableSprite Image => imageCache ??= new(image);
        public List<Vector2> Censures => censures;
    }
}