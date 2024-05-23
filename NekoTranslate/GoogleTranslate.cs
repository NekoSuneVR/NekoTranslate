using Newtonsoft.Json.Linq;

namespace NekoTranslate
{
    public class GoogleTranslateException : Exception
    {
        public GoogleTranslateException(string message) : base(message) { }
    }

    public class GoogleTranslate
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public GoogleTranslate()
        {
        }
        public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
        {
            var url = $"https://clients5.google.com/translate_a/single?dj=1&dt=t&dt=sp&dt=ld&dt=bd&client=dict-chrome-ex&sl={fromLang}&tl={toLang}&q={Uri.EscapeDataString(text)}";
            var response = await httpClient.GetStringAsync(url);
            return JObject.Parse(response).ToString() ?? string.Empty;
        }

    }
}
