using Newtonsoft.Json.Linq;

namespace NekoTranslate
{
    public class MyMemoryException : Exception
    {
        public MyMemoryException(string message) : base(message) { }
    }

    public class MyMemoryTranslate
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string apiKey;
        private const string apiUrl = "https://api.mymemory.translated.net/get";

        public MyMemoryTranslate(string apiKey = null)
        {
            this.apiKey = apiKey;
        }

        public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
        {
            var url = $"{apiUrl}?q={Uri.EscapeDataString(text)}&langpair={fromLang}|{toLang}";
            if (!string.IsNullOrEmpty(apiKey))
            {
                url += $"&key={apiKey}";
            }

            var response = await httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);
            var matches = json["matches"] as JArray;

            if (matches == null || matches.Count == 0)
            {
                throw new MyMemoryException("NO_MATCH");
            }

            var translation = matches[0]["translation"].ToString();
            return translation;
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            var url = $"{apiUrl}?q={Uri.EscapeDataString(text)}&langpair=autodetect|en";
            var response = await httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);
            var matches = json["matches"] as JArray;

            if (matches == null || matches.Count == 0)
            {
                throw new MyMemoryException("NO_MATCH");
            }

            var detectedLanguage = matches[0]["source"].ToString().Split('-')[0];
            return detectedLanguage;
        }
    }
}
