namespace NekoTranslate
{
    public static class Translator
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public static async Task<string> Translate(string fromLang, string toLang, string text, string provider, string apiKey = null)
        {
            switch (provider.ToLower())
            {
                case "google":
                    return await GoogleTranslate(fromLang, toLang, text);
                case "deepl":
                    if (string.IsNullOrEmpty(apiKey))
                        throw new ArgumentException("API key is required for DeepL");
                    return await DeeplTranslate(fromLang, toLang, text, apiKey);
                case "bing":
                    return await BingTranslate(fromLang, toLang, text);
                case "yandex":
                    if (string.IsNullOrEmpty(apiKey))
                        throw new ArgumentException("API key is required for Yandex");
                    return await YandexTranslate(fromLang, toLang, text, apiKey);
                case "mymemory":
                    if (string.IsNullOrEmpty(apiKey))
                        throw new ArgumentException("API key is required for MyMemory");
                    return await MyMemoryTranslate(fromLang, toLang, text, apiKey);
                case "libre":
                    return await LibreTranslate(fromLang, toLang, text);
                case "reverso":
                    return await ReversoTranslate(fromLang, toLang, text);
                case "translate.com":
                    return await TranslateComTranslate(fromLang, toLang, text);
                default:
                    throw new ArgumentException("Unsupported provider");
            }
        }

        private static async Task<string> GoogleTranslate(string fromLang, string toLang, string text)
        {
            var googleTranslator = new GoogleTranslate();
            return await googleTranslator.TranslateAsync(text, fromLang, toLang);
        }

        private static async Task<string> DeeplTranslate(string fromLang, string toLang, string text, string apiKey)
        {
            var deeplTranslator = new DeeplTranslate(apiKey);
            return await deeplTranslator.TranslateAsync(text, fromLang, toLang);
        }

        private static async Task<string> BingTranslate(string fromLang, string toLang, string text)
        {
            var bingTranslator = new BingTranslate(httpClient);
            return await bingTranslator.TranslateAsync(text, fromLang, toLang);
        }

        private static async Task<string> YandexTranslate(string fromLang, string toLang, string text, string apiKey)
        {
            var yandexTranslator = new YandexTranslate(apiKey);
            return await yandexTranslator.TranslateAsync(text, fromLang, toLang);
        }
        private static async Task<string> MyMemoryTranslate(string fromLang, string toLang, string text, string apiKey)
        {
            var myMemoryTranslator = new MyMemoryTranslate(apiKey);
            return await myMemoryTranslator.TranslateAsync(text, fromLang, toLang);
        }
        private static async Task<string> LibreTranslate(string fromLang, string toLang, string text)
        {
            var libreTranslator = new LibreTranslate();
            return await libreTranslator.TranslateAsync(text, fromLang, toLang);
        }

        private static async Task<string> ReversoTranslate(string fromLang, string toLang, string text)
        {
            var reversoTranslator = new ReversoTranslate(httpClient);
            return await reversoTranslator.TranslateAsync(text, fromLang, toLang);
        }
        private static async Task<string> TranslateComTranslate(string fromLang, string toLang, string text)
        {
            var translateComTranslator = new TranslateComTranslate(httpClient);
            var (detectedLanguage, translatedText) = await translateComTranslator.TranslateAsync(text, fromLang, toLang);
            return translatedText;
        }
    }
}
