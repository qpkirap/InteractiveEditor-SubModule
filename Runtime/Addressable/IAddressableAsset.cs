#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;

namespace UnityEngine.AddressableAssets
{
    public interface IAddressableAsset : IDisposable
    {
        string AssetGUID { get; }
    }
}