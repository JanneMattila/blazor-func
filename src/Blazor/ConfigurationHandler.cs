using SharedLibrary;

namespace Blazor
{
    public class ConfigurationHandler
    {
        private static ConfigurationData _configurationData;

        public ConfigurationData Get()
        {
            return _configurationData;
        }

        public void Set(ConfigurationData configurationData)
        {
            _configurationData = configurationData;
        }
    }
}
