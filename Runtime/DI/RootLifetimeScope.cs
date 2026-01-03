using System.Collections.Generic;
using Module.InteractiveEditor.DI;
using Module.Utils.Configs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Module.InteractiveEditor.Runtime.DI
{
    /// <summary>
    /// Корневой LifetimeScope для регистрации всех сервисов приложения.
    /// Должен находиться в Init сцене и быть DontDestroyOnLoad.
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("Конфиги")]
        [SerializeField] private List<BaseConfig> configs = new();
        
        [Header("Адаптеры для авторегистрации")]
        [SerializeField] private List<MonoInjectAdapter> adapters = new();
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Регистрация всех конфигов по их конкретному типу
            foreach (var config in configs)
            {
                if (config == null) continue;
                builder.RegisterInstance(config).AsSelf().AsImplementedInterfaces();
            }
            
            // Авторегистрация через адаптеры
            foreach (var adapter in adapters)
            {
                if (adapter == null) continue;
                adapter.Configure(builder);
            }
            
            // Инициализатор сервисов
            builder.Register<ServiceInitializer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
        
        /// <summary>
        /// Инжектит зависимости в указанный объект.
        /// </summary>
        public void InjectTo(object target)
        {
            Container?.Inject(target);
        }
    }
}
