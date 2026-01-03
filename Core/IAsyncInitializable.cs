using Cysharp.Threading.Tasks;

namespace Module.InteractiveEditor.DI
{
    /// <summary>
    /// Базовый интерфейс для сервисов, требующих асинхронной инициализации.
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Порядок инициализации внутри фазы. Меньшее значение = раньше.
        /// </summary>
        int InitOrder => 0;
        
        /// <summary>
        /// Основная инициализация сервиса.
        /// </summary>
        UniTask Init();
    }
    
    /// <summary>
    /// Интерфейс для пост-инициализации после всех фаз.
    /// </summary>
    public interface IAsyncPostInitializable
    {
        int PostInitOrder => 0;
        UniTask PostInit();
    }
    
    // === Фазы инициализации ===
    // Порядок: Configs → States → Managers → Other → PostInit
    // Внутри каждой фазы сортируется по InitOrder.
    
    /// <summary>
    /// Phase 1: Конфиги. Инициализируются первыми.
    /// </summary>
    public interface IAsyncConfigInitializable : IAsyncInitializable { }
    
    /// <summary>
    /// Phase 2: Состояния (ISavable). Инициализируются после конфигов.
    /// </summary>
    public interface IAsyncStateInitializable : IAsyncInitializable { }
    
    /// <summary>
    /// Phase 3: Менеджеры. Инициализируются после состояний.
    /// </summary>
    public interface IAsyncManagerInitializable : IAsyncInitializable { }
}
