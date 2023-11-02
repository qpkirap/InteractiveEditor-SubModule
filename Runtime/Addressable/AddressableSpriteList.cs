using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.AddressableAssets
{
    public class AddressableSpriteList : IReadOnlyList<AddressableSprite>
    {
        private readonly List<AddressableSprite> convert;

        public AddressableSpriteList(List<AssetReference> images)
        {
            convert = images != null
                ? images.Select(item => new AddressableSprite(item)).ToList()
                : Enumerable.Empty<AddressableSprite>().ToList();
        }
        public IEnumerator<AddressableSprite> GetEnumerator()
        {
            foreach (var image in convert)
            {
                yield return image;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => convert.Count;

        public AddressableSprite this[int index] => convert[index];
    }
}