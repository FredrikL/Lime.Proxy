using System.Configuration;

namespace LimeProxy.Proxy
{
    public interface IConfigurationManagerWrapper
    {
        string AppSettings(string key);
    }

    class ConfigurationManagerWrapper : IConfigurationManagerWrapper
    {
        public string AppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}