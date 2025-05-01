using System.Collections.Generic;

namespace ConfigMaster.Domain.Interfaces
{
    public interface IConfigurationRepository
    {
        Dictionary<string, Dictionary<string, string>> GetValuesByKey(string key);
        Dictionary<string, string> GetValuesBySection(string section);
        Dictionary<string, Dictionary<string, string>> GetAllData();
        void AddSection(string section, Dictionary<string, string> values);
        void UpdateKeysBySection(string section, Dictionary<string, string> values);
        void DeleteKey(string section, string key);
        void DeleteSection(string section);
    }
}
