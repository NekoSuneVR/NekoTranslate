using System.Text;
public class TranslateComTranslate
{
    private readonly HttpClient httpClient;

    private readonly string translateUrl = "https://www.translate.com/translator/ajax_translate";
    private readonly string langDetectUrl = "https://www.translate.com/translator/ajax_lang_auto_detect";

    public TranslateComTranslate(HttpClient client)
    {
        httpClient = client;
    }

    public async Task<(string, string)> TranslateAsync(string text, string fromLang, string toLang)
    {
        if (fromLang == "auto")
        {
            fromLang = await DetectLanguageAsync(text);
        }

        var requestData = new StringContent($"text_to_translate={Uri.EscapeDataString(text)}&source_lang={fromLang}&translated_lang={toLang}&use_cache_only=false", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await httpClient.PostAsync(translateUrl, requestData);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var translatedText = responseContent.Split("translated_text")[1].Split("\"")[2];
            return (fromLang, translatedText);
        }

        return (null, null);
    }

    private async Task<string> DetectLanguageAsync(string text)
    {
        var requestData = new StringContent($"text_to_translate={Uri.EscapeDataString(text)}", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await httpClient.PostAsync(langDetectUrl, requestData);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var detectedLanguage = responseContent.Split("language")[1].Split("\"")[2];
            return detectedLanguage;
        }

        return null;
    }
}
