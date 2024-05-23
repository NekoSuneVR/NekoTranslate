using Newtonsoft.Json.Linq;

namespace NekoTranslate
{
    public class LibreTranslate
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string apiUrl = "https://libretranslate.com";

        public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
        {
            var response = await httpClient.PostAsync($"{apiUrl}/translate", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("q", text),
                new KeyValuePair<string, string>("source", fromLang),
                new KeyValuePair<string, string>("target", toLang)
            }));
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["translatedText"].ToString();
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            var response = await httpClient.PostAsync($"{apiUrl}/detect", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("q", text)
            }));
            var json = JArray.Parse(await response.Content.ReadAsStringAsync());
            return json[0]["language"].ToString();
        }
    }
}
