using System.Collections.Generic;
using Managers.Router.Config.Loading;

namespace Managers.Router.Config
{
    /// <summary>
    /// Интерфейс для конфигов роутера.
    /// Позволяет регистрировать несколько конфигов в DI без конфликтов.
    /// </summary>
    public interface IRouterConfig
    {
        List<SceneData> Scenes { get; }
        List<RoutData> Routs { get; }
        List<LoadingScreenData> Loadings { get; }
    }
}
