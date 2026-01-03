using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module.InteractiveEditor.DI;
using Module.InteractiveEditor.Saves;
using VContainer;

namespace Module.InteractiveEditor.Runtime.DI
{
    /// <summary>
    /// Адаптер для автоматической регистрации сервисов из InteractiveEditor и связанных сборок.
    /// Сканирует сборки и регистрирует все типы, реализующие IAsyncInitializable и его наследников.
    /// </summary>
    public class InteractiveEditorInjectAdapter : MonoInjectAdapter
    {
        public override void Configure(IContainerBuilder builder)
        {
            // Типы для авторегистрации
            var targetInterfaces = new List<Type>
            {
                typeof(IAsyncConfigInitializable),  // Phase 1: Configs
                typeof(IAsyncStateInitializable),   // Phase 2: States (ISavable)
                typeof(IAsyncManagerInitializable), // Phase 3: Managers
                typeof(IAsyncInitializable),        // Базовый интерфейс
                typeof(IAsyncPostInitializable)
            };

            // Собираем типы из всех связанных сборок InteractiveEditor
            var assemblies = new List<Assembly>
            {
                typeof(InteractiveEditorInjectAdapter).Assembly,  // IteractiveEditor
                typeof(Managers.Router.Router).Assembly,          // Router
            };

            var typesToRegister = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .Where(t => targetInterfaces.Any(i => i.IsAssignableFrom(t)))
                    // Exclude SaveNodeItem subclasses - they are data objects created dynamically,
                    // not DI-managed singletons
                    .Where(t => !typeof(SaveNodeItem).IsAssignableFrom(t));
                
                typesToRegister.AddRange(types);
            }

            foreach (var type in typesToRegister)
            {
                builder.Register(type, Lifetime.Singleton)
                    .AsSelf()
                    .AsImplementedInterfaces();
            }
        }
    }
}
