using Module.Utils.Configs;

namespace Provider.Runtime
{
    public interface IConfigsProvider
    {
        TConfig GetConfig<TConfig>() where TConfig : BaseConfig;
    }
}