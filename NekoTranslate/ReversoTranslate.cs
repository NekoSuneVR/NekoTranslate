using System.Text;
using Newtonsoft.Json.Linq;

public class ReversoTranslate
{
    private readonly HttpClient httpClient;

    public ReversoTranslate(HttpClient client)
    {
        httpClient = client;
    }

    public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
    {
        if (fromLang == "auto")
        {
            fromLang = await DetectLanguageAsync(text);
        }

        var requestContent = new JObject
        {
            ["input"] = text,
            ["from"] = fromLang,
            ["to"] = toLang,
            ["format"] = "text",
            ["options"] = new JObject
            {
                ["origin"] = "translation.web",
                ["sentenceSplitter"] = false,
                ["contextResults"] = false,
                ["languageDetection"] = false
            }
        };

        var response = await httpClient.PostAsync(
            "https://api.reverso.net/translate/v1/translation",
            new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseContent);

        return jsonResponse["translation"][0].ToString();
    }

    private async Task<string> DetectLanguageAsync(string text)
    {
        var requestContent = new JObject
        {
            ["input"] = text,
            ["from"] = "eng",
            ["to"] = "fra",
            ["format"] = "text",
            ["options"] = new JObject
            {
                ["origin"] = "translation.web",
                ["sentenceSplitter"] = false,
                ["contextResults"] = false,
                ["languageDetection"] = true
            }
        };

        var response = await httpClient.PostAsync(
            "https://api.reverso.net/translate/v1/translation",
            new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseContent);

        return jsonResponse["languageDetection"]["detectedLanguage"].ToString();
    }
}
