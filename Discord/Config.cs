using Newtonsoft.Json;

namespace SeeleRichPresence.Discord
{
    class config
    {
        public string? Message { get; set; }
        public string[]? Art_Assets { get; set; }

        public static object RPC()
        {
            string json = File.ReadAllText("config.json");
            config? config = JsonConvert.DeserializeObject<config>(json);

            return config;
        }
    }
}