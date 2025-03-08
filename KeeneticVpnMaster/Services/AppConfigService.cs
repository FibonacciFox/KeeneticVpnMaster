using System.IO;
using System.Text.Json;
using KeeneticVpnMaster.Models;

namespace KeeneticVpnMaster.Services
{
    public class AppConfigService
    {
        public AuthConfig Auth { get; set; } = new();
        public UserPreferences UserPreferences { get; set; } = new();

        private static readonly string ConfigFilePath = "config.json";

        public static AppConfigService Load()
        {
            if (!File.Exists(ConfigFilePath))
            {
                var defaultConfig = new AppConfigService();
                defaultConfig.Save();
                return defaultConfig;
            }

            var json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<AppConfigService>(json) ?? new AppConfigService();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}