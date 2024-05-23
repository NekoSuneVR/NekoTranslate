using Newtonsoft.Json.Linq;

namespace NekoTranslate
{
    public class DeeplTranslateException : Exception
    {
        public DeeplTranslateException(string message) : base(message) { }
    }

    public class DeeplTranslate
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string apiKey;

        public DeeplTranslate(string apiKey)
        {
            this.apiKey = apiKey;
        }
        public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
        {
            var url = $"https://api.deepl.com/v2/translate?auth_key={apiKey}&text={Uri.EscapeDataString(text)}&source_lang={fromLang}&target_lang={toLang}";
            var response = await httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (json["code"]?.ToString() != "200")
            {
                throw new DeeplTranslateException(json["message"]?.ToString() ?? "Translation error");
            }

            return json["translations"]?[0]?.ToString() ?? string.Empty;
        }

    }
}
