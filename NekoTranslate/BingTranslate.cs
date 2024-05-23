using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace NekoTranslate
{
    public class BingTranslateException : Exception
    {
        public BingTranslateException(string message) : base(message) { }
    }

    public class BingSessionManager
    {
        private readonly HttpClient _httpClient;
        private string _ig;
        private string _iid;
        private string _key;
        private string _token;
        private HttpRequestMessage _authSession;

        public BingSessionManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            InitializeSession();
        }

        private async void InitializeSession()
        {
            for (int i = 0; i < 3; i++)
            {
                var response = await _httpClient.GetAsync("https://www.bing.com/translator");
                var content = await response.Content.ReadAsStringAsync();

                _ig = Regex.Match(content, "IG:\"(.*?)\"").Groups[1].Value;
                _iid = Regex.Match(content, "data-iid=\"(.*?)\"").Groups[1].Value;
                var helperInfo = Regex.Match(content, "params_AbusePreventionHelper = (.*?);").Groups[1].Value;

                if (!string.IsNullOrEmpty(helperInfo))
                {
                    var helperData = JArray.Parse(helperInfo);
                    _key = helperData[0].ToString();
                    _token = helperData[1].ToString();
                    _authSession = new HttpRequestMessage(HttpMethod.Post, "https://www.bing.com/ttranslatev3");
                    _authSession.Headers.Add("Cookie", response.Headers.GetValues("Set-Cookie"));
                    break;
                }
            }
        }

        public async Task<JObject> SendAsync(string url, Dictionary<string, string> data)
        {
            for (int i = 0; i < 2; i++)
            {
                var content = new FormUrlEncodedContent(data);
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                request.Headers.Add("Cookie", _authSession.Headers.GetValues("Set-Cookie"));

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                var statusCode = jsonResponse.ContainsKey("statusCode") ? (int)jsonResponse["statusCode"] : (int)response.StatusCode;

                if (statusCode == 200)
                    return jsonResponse;

                if (statusCode == 400)
                    InitializeSession();
                else if (statusCode == 429)
                    throw new BingTranslateException("Too many requests");

                throw new BingTranslateException($"Request failed with status code {statusCode}");
            }

            throw new BingTranslateException("Failed to send request after multiple attempts");
        }
    }

    public class BingTranslate
    {
        private readonly BingSessionManager _sessionManager;

        public BingTranslate(HttpClient httpClient)
        {
            _sessionManager = new BingSessionManager(httpClient);
        }

        public async Task<string> TranslateAsync(string text, string fromLang, string toLang)
        {
            var data = new Dictionary<string, string>
            {
                { "text", text },
                { "fromLang", fromLang },
                { "to", toLang }
            };

            var response = await _sessionManager.SendAsync("https://www.bing.com/ttranslatev3", data);
            var translatedText = response[0]["translations"][0]["text"].ToString();

            return translatedText;
        }
    }
}
