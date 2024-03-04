using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StarBot.Helper;
using StarBot.IService;
using StarBot.Model;
using Newtonsoft.Json;

namespace StarBot.Extension
{
    /// <summary>
    /// 日志记录注解类
    /// ActionFilter过滤器的实现
    /// </summary>
    /// <remarks>
    /// 构造函数
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]//定义作用范围含类或方法
    public class LogAttribute(string title) : ActionFilterAttribute
    {
        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title { get; set; } = title;
        private readonly ISysLog _logs = ConfigHelper.GetService<ISysLog>();

        /// <summary>
        /// OnActionExecuted是在Action中的代码执行之后运行的方法。
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            try
            {
                if (context == null) return;
                string url = context.HttpContext.Request.GetAbsoluteUri();
                //获取提交参数，保存日志详细
                string method = context.HttpContext.Request.Method.ToUpper();
                string controller = context.RouteData.Values["Controller"]!.ToString()!;
                string action = context.RouteData.Values["Action"]!.ToString()!;

                string body, form;
                string jsonResult = string.Empty;
                //捕获响应的数据
                if (context.Result is ContentResult result && result.ContentType == "application/json")
                {
                    jsonResult = result.Content!.Replace("\r\n", "").Trim();
                }
                if (context.Result is JsonResult result2)
                {
                    jsonResult = result2.Value?.ToString() ?? "";
                }
                if (context.Result is ObjectResult result3)
                {
                    jsonResult = result3.Value?.ToString() ?? "";
                }
                //捕获响应的数据

                //捕获请求的数据
                try
                {
                    //没有form的情况会报错
                    Dictionary<string, string> formDic = GetRequestForm(context.HttpContext.Request.Form);
                    form = JsonConvert.SerializeObject(formDic);
                }
                catch
                {
                    form = "{}";
                }

                if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method))
                {
                    using var reader = new StreamReader(context.HttpContext.Request.Body);
                    //需要使用异步方式才能获取
                    body = reader.ReadToEndAsync().GetAwaiter().GetResult();
                    if (string.IsNullOrEmpty(body))
                    {
                        body = context.HttpContext.Request.QueryString.Value ?? "";
                    }
                }
                else
                {
                    body = context.HttpContext.Request.QueryString.Value ?? "";
                }
                body = string.IsNullOrEmpty(body) ? "{}" : body;
                //捕获请求的数据

                //记录数据库
                string str = controller + "." + action + "()," + method + " form:" + form + " body:" + body + " response:" + jsonResult;
                _logs.WriteLog(str);
            }
            catch (Exception exc)
            {
                throw new Exception(exc.ToString());
            }
        }

        /// <summary>
        /// 把Form集合转为字典集合
        /// </summary>
        /// <param name="formCollection">Form集合</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetRequestForm(IFormCollection formCollection)
        {
            Dictionary<string, string> sArray = [];
            foreach (var item in formCollection)
            {
                sArray.Add(item.Key, item.Value!);
            }
            return sArray;
        }
    }
}
