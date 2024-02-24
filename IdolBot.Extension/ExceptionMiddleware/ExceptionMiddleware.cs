using Microsoft.AspNetCore.Http;
using IdolBot.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IdolBot.IService;

namespace IdolBot.Extension
{
    /// <summary>
    /// 异常处理中间件
    /// </summary>
    /// <remarks>
    /// 构造函数
    /// </remarks>
    public class ExceptionMiddleware(RequestDelegate next, ILogs logs)
    {
        /// <summary>
        /// 管道代理对象
        /// </summary>
        private readonly RequestDelegate _next = next;
        private readonly ILogs _logs = logs;

        /// <summary>
        /// 调用管道
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            string msg = "";
            try
            {
                await _next(context);
            }
            catch (Exception ex) //发生异常
            {
                context.Response.StatusCode = 500;
                msg = ex.Message;
            }
            finally
            {
                if (context.Response.StatusCode != 200)
                {
                    msg = context.Response.StatusCode switch
                    {
                        401 => "用户未授权",
                        403 => "API未授权",
                        404 => "未找到资源",
                        405 => "请求方法不匹配",
                        500 => msg,
                        502 => "请求错误",
                        _ => "未知错误:" + msg,
                    };
                    await HandleExceptionAsync(context, context.Response.StatusCode, msg);
                }
            }
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, int statusCode, string msg)
        {
            ApiResult apiResult = new()
            {
                Code = statusCode,
                Msg = msg,
                Success = false
            };
            context.Response.ContentType = "application/json;charset=utf-8";
            JsonSerializerSettings settings = new()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResult, settings));
            if (statusCode >= 500)
            {
                var code = statusCode.ToString();
                var url = context.Request.Path;
                await _logs.WriteLog(code, msg, Enums.LogLevel.ERROR, url);
            }
        }
    }
}
