using Microsoft.CognitiveServices.Speech;
using NullLib.CommandLine;
using NullLib.ConsoleEx;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace Null.TextSpeech
{
    public class AppCommands : CommandHome    // 这里是程序支持的指令
    {
        [Command]
        public string? ShowLang()
        {
            return Program.AppConfig?.TextSpeech.CurLang;
        }
        [Command]
        public string? ShowVoice()
        {
            return Program.AppConfig?.TextSpeech.CurVoice;
        }
        [Command]
        public Dictionary<string, string>? ShowAllLangs()
        {
            KeyValuePair<string, string>[]? keyValuePairs = Program.AppConfig?.TextSpeech.AllLangs.Keys.Select(v => new KeyValuePair<string, string>(v, new CultureInfo(v).DisplayName)).ToArray();
            if (keyValuePairs != null)
                return new Dictionary<string, string>(keyValuePairs);
            else
                return null;
        }
        [Command]
        public string[]? ShowAllVoices()
        {
            string? lang = ShowLang();
            return lang != null ? Program.AppConfig?.TextSpeech.AllLangs[lang].ToArray() : null;
        }
        [Command]
        public bool SetLang(string lang)
        {
            if (Program.AppConfig?.TextSpeech.AllLangs.ContainsKey(lang) == true)
            {
                Program.AppConfig.TextSpeech.CurLang = lang;
                return true;
            }

            return false;
        }
        [Command]
        public bool SetVoice(string voice)
        {
            if (Program.AppConfig != null)
            {
                string? lang = ShowLang();
                if (lang != null && Program.AppConfig.TextSpeech.AllLangs.ContainsKey(lang))
                {
                    List<string> list = Program.AppConfig.TextSpeech.AllLangs[lang];
                    if (list.Contains(voice))
                    {
                        Program.AppConfig.TextSpeech.CurVoice = voice;
                        return true;
                    }
                }
            }

            return false;
        }
        [Command]
        public string? ShowKey()
        {
            return Program.AppConfig?.Subcription.ApiKey;
        }
        [Command]
        public bool SetKey(string key)
        {
            if (Program.AppConfig != null)
            {
                Program.AppConfig.Subcription.ApiKey = key;
                return true;
            }
            return false;
        }
        [Command]
        public string? ShowRegion()
        {
            return Program.AppConfig?.Subcription.Region;
        }
        [Command]
        public bool SetRegion(string region)
        {
            if (Program.AppConfig != null)
            {
                Program.AppConfig.Subcription.Region = region;
                return true;
            }
            return false;
        }



        [Command]
        public bool SaveConfig()
        {
            try
            {
                File.WriteAllText(Program.AppConfigPath, JsonSerializer.Serialize(Program.AppConfig, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                }));

                return true;
            }
            catch
            {
                return false;
            }
        }
        [Command]
        public bool Reload()
        {
            return ReloadConfig() & ReloadSpeech();
        }
        [Command]
        public bool ReloadConfig()
        {
            try
            {
                Program.InitConfig();
                return true;
            }
            catch { return false; }

        }
        [Command]
        public bool ReloadSpeech()
        {
            try
            {
                Program.InitSpeech();
                return true;
            }
            catch { return false; }
        }
        [Command]
        public bool EditConfig()
        {
            try
            {
                Process.Start(new ProcessStartInfo(Program.AppConfigPath)
                {
                    UseShellExecute = true,
                });

                return true;
            }
            catch { return false; }
        }

        [Command(CommandAlias = "<")]
        public string? CapVoice()
        {
            using SpeechRecognizer recognizer = new SpeechRecognizer(Program.SpeechConfig);
            SpeechRecognitionResult result = recognizer.RecognizeOnceAsync().Result;
            return result.Text;
        }

        [Command(CommandAlias = "<<")]
        public bool CapAllVoice()
        {
            try
            {
                using SpeechRecognizer recognizer = new SpeechRecognizer(Program.SpeechConfig);
                recognizer.Recognizing += (s, e) => Console.WriteLine(e.Result.Text);
                recognizer.Recognized += (s, e) => Console.WriteLine(e.Result.Text);
                _ = recognizer.StartContinuousRecognitionAsync();
                Console.ReadKey();
                recognizer.StopKeywordRecognitionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Command]
        public void Exit()
        {
            Environment.Exit(0);
        }
    }
}