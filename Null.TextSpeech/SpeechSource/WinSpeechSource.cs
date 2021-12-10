using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace Null.TextSpeech.SpeechSource
{
    public class WinSpeechSource : ISpeechSource
    {
        public WinSpeechSource(SpeechSynthesizer synthesizer)
        {

        }

        public string[] GetAllLanguages()
        {
            throw new NotImplementedException();
        }

        public string[] GetAllVoices()
        {
            throw new NotImplementedException();
        }

        public string GetLanguage()
        {
            throw new NotImplementedException();
        }

        public string GetVoice()
        {
            throw new NotImplementedException();
        }

        public bool SetLanguage(string language)
        {
            throw new NotImplementedException();
        }

        public bool SetVoice(string[] voices)
        {
            throw new NotImplementedException();
        }

        public object Speak(string text)
        {
            throw new NotImplementedException();
        }

        public Task<object> SpeakAsync(string text)
        {
            throw new NotImplementedException();
        }
    }
}
