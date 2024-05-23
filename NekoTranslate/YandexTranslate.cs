using Newtonsoft.Json.Linq;

namespace NekoTranslate
{
    public class YandexTranslateException : Exception
    {
        public YandexTranslateException(string message) : base(message) { }
    }

    public class YandexTranslate
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string apiUrl = "https://translate.yandex.net/api/v1.5/tr.json/translate";

        private readonly string apiKey;

        public YandexTranslate(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
        {
            var url = $"{apiUrl}?key={apiKey}&text={Uri.EscapeDataString(text)}&lang={fromLang}-{toLang}";
            var response = await httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (json["code"]?.ToString() != "200")
            {
                throw new YandexTranslateException(json["message"]?.ToString() ?? "Translation error");
            }

            return json["text"]?[0]?.ToString() ?? string.Empty;
        }
    }
}
