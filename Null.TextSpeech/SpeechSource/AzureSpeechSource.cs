using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace Null.TextSpeech.SpeechSource
{
    internal class AzureSpeechSource : ISpeechSource
    {
        public AzureSpeechSource(SpeechConfig config)
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
