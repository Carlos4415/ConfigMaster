using Microsoft.Extensions.Configuration;
using ConfigMaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConfigMaster.Infrastructure.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _filePath;

        public ConfigurationRepository()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddIniFile("Configuration/config.ini");

            _configuration = builder.Build();
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration/config.ini");
        }

        public Dictionary<string, Dictionary<string, string>> GetValuesByKey(string key)
        {
            var allSections = new Dictionary<string, Dictionary<string, string>>();

            foreach (var section in _configuration.GetChildren())
            {
                var values = new Dictionary<string, string>();
                foreach (var child in section.GetChildren())
                {
                    if (child.Key == key)
                    {
                        values[child.Key] = child.Value;
                        allSections[section.Key] = values;
                    }
                }
            }

            return allSections;
        }

        public Dictionary<string, string> GetValuesBySection(string section)
        {
            var configurationSection = _configuration.GetSection(section);

            if (!configurationSection.Exists())
            {
                return null;
            }

            var values = new Dictionary<string, string>();
            foreach (var child in configurationSection.GetChildren())
            {
                values[child.Key] = child.Value;
            }

            return values;
        }

        public Dictionary<string, Dictionary<string, string>> GetAllData()
        {
            var allSections = new Dictionary<string, Dictionary<string, string>>();

            foreach (var section in _configuration.GetChildren())
            {
                var values = new Dictionary<string, string>();
                foreach (var child in section.GetChildren())
                {
                    values[child.Key] = child.Value;
                }
                allSections[section.Key] = values;
            }

            return allSections;
        }

        public void AddSection(string section, Dictionary<string, string> values)
        {
            if (SectionAlreadyExists(section))
            {
                throw new InvalidOperationException($"The section already exists in the configuration file.");
            }

            using (StreamWriter writer = new StreamWriter(_filePath, true))
            {
                writer.WriteLine();
                writer.WriteLine($"[{section}]");

                foreach (var kvp in values)
                {
                    writer.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }
        }

        public void UpdateKeysBySection(string section, Dictionary<string, string> values)
        {

            if (!SectionAlreadyExists(section))
            {
                throw new KeyNotFoundException($"The section was not found in the configuration file.");
            }

            var lines = File.ReadAllLines(_filePath).ToList();
            var sb = new StringBuilder();
            bool withinSection = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (withinSection)
                    {
                        withinSection = false;
                    }

                    if (line.Equals($"[{section}]"))
                    {
                        withinSection = true;
                    }
                }

                 if (!withinSection)
                {
                    sb.AppendLine(line);
                }
            }

            using (var writer = new StreamWriter(_filePath, false))
            {
                writer.Write(sb.ToString());
                writer.WriteLine($"[{section}]");

                foreach (var kvp in values)
                {
                    writer.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }
        }

        private bool SectionAlreadyExists(string section)
        {
            var existingSections = _configuration.GetChildren().Select(section => section.Key);
            return existingSections.Contains(section);
        }

        public void DeleteKey(string section, string key)
        {
            if (!SectionAlreadyExists(section))
            {
                throw new KeyNotFoundException($"The section was not found in the configuration file.");
            }

            var lines = File.ReadAllLines(_filePath).ToList();
            var sb = new StringBuilder();
            bool withinSection = false;
            bool keyFound = false;
            int keyCount = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (withinSection)
                    {
                        withinSection = false;
                    }

                    if (line.Equals($"[{section}]"))
                    {
                        withinSection = true;
                    }
                }

                if (withinSection)
                {
                    if (line.Contains('='))
                    {
                        var configurationKey = line.Split('=')[0].Trim();
                        keyCount++;

                        if (key.Equals(configurationKey, StringComparison.OrdinalIgnoreCase))
                        {
                            keyFound = true;
                            continue;
                        }
                    }
                }

                sb.AppendLine(line);
            }

            if (!keyFound)
            {
                throw new KeyNotFoundException($"The key was not found in the section in the configuration file.");
            }

            if (keyCount > 0)
            {
                using (var writer = new StreamWriter(_filePath, false))
                {
                    writer.Write(sb.ToString());
                }

                if (keyCount == 1)
                {
                    DeleteSection(section);
                }
            }
        }

        public void DeleteSection(string section)
        {
            if (!SectionAlreadyExists(section))
            {
                throw new KeyNotFoundException($"The section was not found in the configuration file.");
            }

            var lines = File.ReadAllLines(_filePath).ToList();
            var sb = new StringBuilder();
            bool withinSection = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (line.Equals($"[{section}]"))
                    {
                        withinSection = true;
                        continue;
                    }

                    withinSection = false;
                }

                if (!withinSection)
                {
                    sb.AppendLine(line);
                }
            }

            using (var writer = new StreamWriter(_filePath, false))
            {
                writer.Write(sb.ToString());
            }
        }
    }
}
