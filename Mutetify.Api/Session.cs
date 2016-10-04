using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Mutetify.Api.Exceptions;

namespace Mutetify.Api
{
    public class Session
    {
        private const string LOCAL_URL = "https://local.spotilocal.com";
        private const string OAUTH_URL = "https://open.spotify.com/token";
        private string _token;
        private string _csrf;
        private string _port;
        private string _spotifyVersion;
        private string _spotifyClientVersion;
        private HttpClient _client;

        /// <summary>
        /// Initialize the handshake with the local server
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            await RequestToken();
            await HandShakeWebHelper();
            await RequestCSRF();
        }

        /// <summary>
        /// Get token from spotify
        /// </summary>
        /// <returns></returns>
        private async Task RequestToken()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new System.Uri(OAUTH_URL)
            };
            JObject response = await SendRequest(request);
            _token = response.GetValue("t").Value<string>();
        }

        /// <summary>
        /// Get CSRF
        /// </summary>
        /// <returns></returns>
        private async Task RequestCSRF()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new System.Uri($"{LOCAL_URL}:{_port}/simplecsrf/token.json")
            };
            request.Headers.TryAddWithoutValidation("Origin", "https://open.spotify.com");
            JObject response = await SendRequest(request);
            _csrf = response.GetValue("token").Value<string>();
        }

        /// <summary>
        /// Initialize the local spotify server configuration
        /// </summary>
        /// <returns></returns>
        private async Task HandShakeWebHelper()
        {
            JObject response = null;
            for (int i = 4371; i < 4380; ++i)
            {
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = new HttpMethod("GET"),
                    RequestUri =
                    new System.Uri($"{LOCAL_URL}:{i}/service/version.json?service=remote")
                };
                try
                {
                    response = await SendRequest(request);
                    _port = i.ToString();
                    _spotifyVersion = response.GetValue("version").Value<string>();
                    _spotifyClientVersion = response.GetValue("client_version").Value<string>();
                    return;
                }
                catch (HttpRequestException)
                {
                    continue;
                }
            }
            throw new WebServiceNotFoundException("Web service instance is not running");
        }

        /// <summary>
        /// Get status of the session
        /// </summary>
        public async Task<SessionStatus> Status()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri =
                new System.Uri($"{LOCAL_URL}:{_port}/remote/status.json?csrf={_csrf}&oauth={_token}")
            };
            JObject response = await SendRequest(request);
            if (response.Property("error") == null)
            {
                SessionStatus status = new SessionStatus()
                {
                    PlayEnabled = (bool)response.Property("play_enabled").Value,
                    NextEnabled = (bool)response.Property("next_enabled").Value,
                    PreviousEnabled = (bool)response.Property("prev_enabled").Value,
                    Running = (bool)response.Property("running").Value,
                };
                return status;
            }
            JObject error = (JObject)response.Property("error").Value;
            throw new ApiException($"{error.Property("message").Value}");
        }

        /// <summary>
        /// Send a request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<JObject> SendRequest(HttpRequestMessage request)
        {
            HttpResponseMessage httpResponse = await _client.SendAsync(request);
            string response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JObject>(response);
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public Session()
        {
            _port = string.Empty;
            _csrf = string.Empty;
            _token = string.Empty;
            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip |
                System.Net.DecompressionMethods.Deflate;
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.
                TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch, br");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");
        }
    }
}
