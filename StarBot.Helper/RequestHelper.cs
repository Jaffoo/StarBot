using System.Net;

namespace Common
{
    public class RequestHelper
    {
        private HttpClient? _httpClient;

        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string>? Headers { get; set; }
        /// <summary>
        /// cookie
        /// </summary>
        public CookieContainer? Cookie { get; set; }
        /// <summary>
        /// 代理
        /// </summary>
        public WebProxy? Proxy { get; set; }

        /// <summary>
        /// url通用前缀
        /// </summary>
        public string BaseUrl { get; set; } = "";

        /// <summary>
        /// 数据类型
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// 组装请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="body">请求体</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Send(string url, HttpMethod method, string body = "")
        {
            var httpClientHandler = new HttpClientHandler();
            if (Cookie != null)
            {
                httpClientHandler.CookieContainer = Cookie;
                httpClientHandler.UseCookies = true;
            }
            if (Proxy != null)
            {
                httpClientHandler.Proxy = Proxy;
                httpClientHandler.UseProxy = true;
            }

            _httpClient = new(httpClientHandler);
            if (Headers != null)
            {
                foreach (var item in Headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(new Uri(BaseUrl), url)
            };

            if (method == HttpMethod.Post)
            {
                request.Content = new StringContent(body);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType!);
            }
            return await _httpClient.SendAsync(request);
        }
    }
}
