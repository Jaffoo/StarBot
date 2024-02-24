using Microsoft.AspNetCore.Http;
using System.Text;

namespace IdolBot.Extension
{

    /// <summary>
    /// HttpRequest扩展
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 获取完整URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        public static string GetDoMain(this HttpRequest request)
        {
            string scheme = request.Scheme;
            StringBuilder strRequest = new();
            //在nginx 配置location处加上 proxy_set_header X-Forwarded-Scheme  $scheme
            string forwardedScheme = request.Headers["X-Forwarded-Scheme"].ToString() ?? "";
            if (!string.IsNullOrWhiteSpace(forwardedScheme))
            {
                scheme = forwardedScheme;
            }

            strRequest
            .Append(scheme)
            .Append("://")
            .Append(request.Host)
            .ToString();
            return strRequest.ToString();
        }

        /// <summary>
        /// 获取相对URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRelativeUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        /// <summary>
        /// 获取request客户端类型 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetUserAgent(this HttpRequest request)
        {
            return request.Headers.UserAgent.ToString() ?? "";
        }
    }
}
