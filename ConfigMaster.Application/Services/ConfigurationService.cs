using ConfigMaster.Domain.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace ConfigMaster.Application.Services
{
    public class ConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public Dictionary<string, Dictionary<string, string>> GetValuesByKey(string key)
        {
            return _configurationRepository.GetValuesByKey(key);
        }

        public Dictionary<string, string> GetValuesBySection(string section)
        {
            return _configurationRepository.GetValuesBySection(section);
        }

        public Dictionary<string, Dictionary<string, string>> GetAllData()
        {
            return _configurationRepository.GetAllData();
        }

        public void AddSection(string section, Dictionary<string, string> values)
        {
            _configurationRepository.AddSection(section, values);
        }

        public void UpdateKeysBySection(string section, Dictionary<string, string> values)
        {
            _configurationRepository.UpdateKeysBySection(section, values);
        }

        public void DeleteKey(string section, string key)
        {
            _configurationRepository.DeleteKey(section, key);
        }

        public void DeleteSection(string section)
        {
            _configurationRepository.DeleteSection(section);
        }
    }
}
