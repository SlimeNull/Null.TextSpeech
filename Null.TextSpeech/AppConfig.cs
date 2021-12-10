using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Null.TextSpeech
{
    using SpeechVoice = String;
    public partial class AppConfig
    {
        public SubcriptionConfig Subcription { get; set; } = new SubcriptionConfig();
        public TextSpeechConfig TextSpeech { get; set; } = new TextSpeechConfig();
        public class SubcriptionConfig
        {
            public string ApiKey { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
        }
        public class TextSpeechConfig
        {
            public string CurLang { get; set; } = string.Empty;
            public string CurVoice { get; set; } = string.Empty;
            public Dictionary<string, List<SpeechVoice>> AllLangs { get; set; } = new Dictionary<SpeechVoice, List<SpeechVoice>>();
        }
    }
}
