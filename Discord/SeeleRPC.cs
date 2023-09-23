using DiscordRPC;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace SeeleRichPresence.Discord
{
    class SeeleRPC
    {
        private static readonly string game = "nemuPlayer";
        private static readonly string token = "1135955419974602772";
        private static bool Initialized = false;
        private static readonly DiscordRpcClient client = new(token);

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        private static void SetAssets(string description_name, string trimeed)
        {
            Console.WriteLine(RemoveDiacritics(trimeed));
            if (description_name.Length != 0)
            {
                if (description_name == "MuMu Player")
                {
                    presence.Details = "Running MuMu Player";
                    presence.Assets = new Assets()
                    {
                        LargeImageText = description_name,
                        LargeImageKey = "mumu",
                    };
                }
                else if (description_name != "MuMu Player" && ReadAssets(RemoveDiacritics(trimeed)) == false)
                {
                    presence.Details = $"Playing Game {description_name}";
                    presence.Assets = new Assets()
                    {
                        LargeImageText = description_name,
                        LargeImageKey = "mumu",

                    };
                }

                else
                {
                    presence.Details = $"Playing Game {description_name}";
                    presence.Assets = new Assets()
                    {
                        LargeImageText = description_name,
                        LargeImageKey = RemoveDiacritics(trimeed),
                        SmallImageText = "MuMu Player",
                        SmallImageKey = "mumu",
                    };
                }
            }
            else
            {
                presence.Details = $"Closing MuMu Player";
                presence.Assets = new Assets()
                {
                    LargeImageText = "Closing MuMu Player",
                    LargeImageKey = "mumu",
                };
            }
            
        }
        private static bool ReadAssets(string assets_name)
        {
            config config = (config)config.RPC();
            string[] asset = config.Art_Assets;
            if(asset.Contains(assets_name)){
                return true;
            }
            return false;
        }
        private static readonly RichPresence presence = new()
        {
            
            Timestamps = Timestamps.Now,
            Assets = new Assets()
            {

                LargeImageText = "MuMu Player",
                LargeImageKey = "mumu",
            }
        };

        private static void RPCStart()
        {
            if (!Initialized)
            {
                client.Initialize();
                Initialized = true;
            }

            config config = (config)config.RPC();

            presence.State = config.Message;

            client.SetPresence(presence);
        }

        private static void Cancel()
        {
            if (client != null && client.IsInitialized)
            {
                client.ClearPresence();
            }
        }

        public static void Start()
        {

            var isRunning = false;
            var description_name = "MuMu Player";
            var last_desc_name = "";
            var desc_trim = "";

            while (true)
            {
                Process[] mumu = Process.GetProcessesByName(game);
                foreach(Process p in mumu)
                {
                   
                    description_name = p.MainWindowTitle.ToString();
                    if (description_name != last_desc_name)
                    {
                        last_desc_name= description_name;
                        desc_trim = String.Concat(last_desc_name.Where(c => !Char.IsWhiteSpace(c)));
                        desc_trim = desc_trim.ToLower();
                        SetAssets(last_desc_name,desc_trim);
                    }
                    
                }

                if (mumu.Length > 0 && isRunning)
                {
                    
                    if(presence.Timestamps == null)
                    {
                        presence.Timestamps = Timestamps.Now;
                    }
                    RPCStart();
                    isRunning = true;
                }
                else if(mumu.Length == 0 && presence.Timestamps != null)
                {
                    presence.Timestamps = null;
                }
                else
                {
                    Cancel();
                    isRunning = true;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
