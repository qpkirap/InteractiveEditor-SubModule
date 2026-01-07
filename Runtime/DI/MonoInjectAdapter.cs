using UnityEngine;
using VContainer;

namespace Module.InteractiveEditor.DI
{
    /// <summary>
    /// Базовый адаптер для конфигурации DI контейнера.
    /// Позволяет декларативно регистрировать сервисы без изменения RootLifetimeScope.
    /// </summary>
    public abstract class MonoInjectAdapter : MonoBehaviour
    {
        public abstract void Configure(IContainerBuilder builder);
    }
}
