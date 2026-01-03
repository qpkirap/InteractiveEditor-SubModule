using System;
using Module.InteractiveEditor.DI;

namespace Module.InteractiveEditor.Saves
{
    /// <summary>
    /// Интерфейс для сохраняемых состояний.
    /// Реализует IAsyncStateInitializable для автоматической инициализации.
    /// </summary>
    public interface ISavable : IEquatable<ISavable>, IAsyncStateInitializable
    {
        string SaveKey { get; }
        void PostLoad();
        void Reset();
    }
}