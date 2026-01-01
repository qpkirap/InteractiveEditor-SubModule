#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityEngine.AddressableAssets
{
    public interface IAddressableAsset : IDisposable
    {
        string AssetGUID { get; }
        UniTask PreloadAsync(CancellationToken token = default);
        UniTask Release();
    }
}