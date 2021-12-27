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
        private static bool networkAvailable = true;

        public static CommandObject<AppCommands> AppCommands { get; } = new CommandObject<AppCommands>();  // 因为这个 CommandObject 的泛型参数是 AppCommands 类, 所以调用的方法也是在这个类中定义的

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
            SpeechConfig = SpeechConfig.FromSubscription(AppConfig?.Subcription?.ApiKey, AppConfig?.Subcription?.Region);
            SpeechConfig.SpeechSynthesisLanguage = AppConfig?.TextSpeech.CurLang;
            SpeechConfig.SpeechSynthesisVoiceName = AppConfig?.TextSpeech.CurVoice;
            SpeechConfig.SpeechRecognitionLanguage = AppConfig?.TextSpeech.CurLang;
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


#if DEBUG

        public static async Task<string?> SpeechRecognizeTest()
        {
            Console.WriteLine("Start recognizing");
            SpeechRecognizer speechRecognizer = new SpeechRecognizer(SpeechConfig);
            SpeechRecognitionResult rst = await speechRecognizer.RecognizeOnceAsync();
            return rst.Text;
        }
        
#endif

        public static void SpeakText(string text)
        {
#if DEBUG
            //NetworkAvailable = false;
#endif
            if (NetworkAvailable)
            {
                Task.Run(async () =>
                {
                    using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(SpeechConfig);
                    using SpeechSynthesisResult? result = await speechSynthesizer.SpeakTextAsync(text);
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
#if DEBUG
            string? rrst = SpeechRecognizeTest().GetAwaiter().GetResult();
            Console.WriteLine(rrst);
#endif
            while (true)
            {
                Console.Write(">>> ");
                string? text = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (text.StartsWith('/'))
                    {
                        if (AppCommands.TryExecuteCommand(text[1..], true, out object rst))   // 执行指令
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
