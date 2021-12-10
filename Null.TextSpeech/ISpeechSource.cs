using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Null.TextSpeech
{
    internal interface ISpeechSource
    {
        public object Speak(string text);
        public Task<object> SpeakAsync(string text);
        public string[] GetAllLanguages();
        public string[] GetAllVoices();
        public bool SetLanguage(string language);
        public bool SetVoice(string[] voices);
        public string GetLanguage();
        public string GetVoice();
    }
}
