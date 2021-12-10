using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Net;
using Microsoft.CognitiveServices.Speech;
using NullLib.CommandLine;
using System.Collections.ObjectModel;

namespace Null.TextSpeech
{
    class Program
    {
        static Program()
        {
            Console.WriteLine("Null.TextSpeech, by SlimeNull, powerd by Microsoft Azure service.");
            Console.WriteLine();

            InitConfig();
            InitSpeech();
            CheckNetwork();   // 新线程检查
        }

        static JsonSerializerOptions StdoJsonOption = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string AppConfigPath = "AppConfig.json";
        public static AppConfig? AppConfig;
        public static SpeechConfig? SpeechConfig;
        public static SpeechSynthesizer? SpeechSynthesizer;
        private static bool networkAvailable = true;

        public static CommandObject<AppCommands> AppCommands { get; } = new CommandObject<AppCommands>();

        public static bool NetworkAvailable
        {
            get => networkAvailable;
            private set
            {
                if (value == networkAvailable)
                    return;

                if (value)
                    Console.Error.WriteLine("You are online now, using Azure text to speech engine.");
                else
                    Console.Error.WriteLine("Your are offline now, using local text to speech engine. config file is not available.");
                networkAvailable = value;
            }
        }
        public static void InitConfig()
        {
            if (!File.Exists(AppConfigPath))
            {
                AppConfig = new AppConfig();
                File.WriteAllText(AppConfigPath, JsonSerializer.Serialize(AppConfig));
            }
            else
            {
                using FileStream fs = File.OpenRead(AppConfigPath);
                AppConfig = JsonSerializer.Deserialize<AppConfig>(fs) ?? new AppConfig();
                if (AppConfig.TextSpeech.CurLang == null)
                {
                    AppConfig.TextSpeech.CurLang = CultureInfo.CurrentCulture.Name;
                }
                if (AppConfig.TextSpeech.CurVoice == null)
                {
                    if (AppConfig.TextSpeech.AllLangs.TryGetValue(AppConfig.TextSpeech.CurLang, out List<string>? voices) && voices.Count > 0)
                    {
                        AppConfig.TextSpeech.CurVoice = voices[0];
                    }
                }
            }
        }
        public static void InitSpeech()
        {
            if (SpeechSynthesizer != null)
                SpeechSynthesizer.Dispose();

            SpeechConfig = SpeechConfig.FromSubscription(AppConfig?.Subcription?.ApiKey, AppConfig?.Subcription?.Region);
            SpeechConfig.SpeechSynthesisLanguage = AppConfig?.TextSpeech.CurLang;
            SpeechConfig.SpeechSynthesisVoiceName = AppConfig?.TextSpeech.CurVoice;
            SpeechSynthesizer = new SpeechSynthesizer(SpeechConfig);
        }
        public static async Task CheckNetwork()
        {
            const string BingUri = "bing.com";

            try
            {
                IPAddress[] addresses = await Dns.GetHostAddressesAsync(BingUri);
                NetworkAvailable = addresses.Length > 0;
            }
            catch { NetworkAvailable = false; }
        }

        public static void SpeakText(string text)
        {
#if DEBUG
            //NetworkAvailable = false;
#endif
            if (NetworkAvailable)
            {
                if (SpeechSynthesizer != null)
                {
                    Task.Run(async () =>
                    {
                        using SpeechSynthesisResult? result = await SpeechSynthesizer.SpeakTextAsync(text);
                        if (result?.Reason != ResultReason.SynthesizingAudioCompleted)
                        {
                            if (result?.Reason == ResultReason.Canceled)
                            {
                                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                                Console.Error.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                                if (cancellation.Reason == CancellationReason.Error)
                                {
                                    Console.Error.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                    Console.Error.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                                }
                            }
                        }
                    });
                }
            }
            else
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    System.Speech.Synthesis.SpeechSynthesizer speechSynthesizer = new();
                    ReadOnlyCollection<System.Speech.Synthesis.InstalledVoice> voices = speechSynthesizer.GetInstalledVoices(new CultureInfo(AppConfig?.TextSpeech.CurLang));
                    if (voices.Count > 0)
                    {
                        System.Speech.Synthesis.InstalledVoice voice = voices.First();
                        speechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
                        speechSynthesizer.SpeakAsync(text);
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(">>> ");
                string? text = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (text.StartsWith('/'))
                    {
                        if (AppCommands.TryExecuteCommand(text[1..], true, out object rst))
                        {
                            if (rst != null)
                                Console.WriteLine(JsonSerializer.Serialize(rst, StdoJsonOption));
                        }
                        else
                        {
                            Console.Error.WriteLine("FAILED: Type /help for help");
                        }
                    }
                    else
                    {
                        SpeakText(text);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
