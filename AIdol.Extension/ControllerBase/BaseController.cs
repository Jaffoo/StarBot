using Microsoft.AspNetCore.Mvc;
using AIdol.Model;

namespace AIdol.Extension
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]

    public class BaseController : Controller
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        protected static ApiResult Success(string msg = "操作成功")
        {
            ApiResult apiResult = new()
            {
                Success = true,
                Code = 200,
                Msg = msg
            };
            return apiResult;
        }
        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected static ApiResult Success(dynamic data, string msg = "操作成功")
        {
            ApiResult apiResult = new()
            {
                Success = true,
                Code = 200,
                Msg = msg,
                Data = data
            };
            return apiResult;
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        protected static ApiResult Failed(string msg = "操作失败")
        {
            ApiResult apiResult = new()
            {
                Success = false,
                Code = 200,
                Msg = msg
            };
            return apiResult;
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        protected static ApiResult Error(string msg = "系统错误")
        {
            ApiResult apiResult = new()
            {
                Success = false,
                Code = 500,
                Msg = msg
            };
            return apiResult;
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static ApiResult AjaxResult(bool b)
        {
            if (b)
            {
                return Success("操作成功！");
            }
            else
            {
                return Failed("操作失败！");
            }
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="b">是否成功</param>
        /// <param name="msg">失败提示信息</param>
        /// <returns></returns>
        protected static ApiResult AjaxResult(bool b, string msg)
        {
            if (b)
            {
                return Success("操作成功！");
            }
            else
            {
                return Failed($"操作失败,原因:{msg}");
            }
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static ApiResult AjaxResult(int b)
        {
            if (b>0)
            {
                return Success("操作成功！");
            }
            else
            {
                return Failed("操作失败！");
            }
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="b"></param>
        /// <param name="msg">失败提示信息</param>
        /// <returns></returns>
        protected static ApiResult AjaxResult(int b, string msg)
        {
            if (b > 0)
            {
                return Success("操作成功！");
            }
            else
            {
                return Failed($"操作失败,原因:{msg}");
            }
        }

        /// <summary>
        /// 列表请求结果
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="count">记录数</param>
        /// <returns></returns>
        protected static ApiResult ListResult(dynamic data, int count)
        {
            ApiResult apiResult = new()
            {
                Success = true,
                Msg = "",
                Data = data,
                Count = count
            };
            return apiResult;
        }

        /// <summary>
        /// 数据请求结果
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected static ApiResult DataResult(dynamic data)
        {
            ApiResult apiResult = new()
            {
                Success = true,
                Msg = "",
                Data = data,
                Code = 200
            };
            return apiResult;
        }

        /// <summary>
        /// 未授权
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        protected ApiResult Unauthorized(string msg)
        {
            ApiResult apiResult = new()
            {
                Success = false,
                Code = 304,
                Msg = msg
            };
            Response.StatusCode = 304;
            return apiResult;
        }

        /// <summary>
        /// 获取缩略图路径
        /// </summary>
        /// <param name="imageUrl">图片地址</param>
        /// <param name="thumbName">缩略图标识</param>
        /// <returns></returns>
        protected static string GeThumbImage(string imageUrl, string thumbName)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || string.IsNullOrWhiteSpace(thumbName))
                return string.Empty;

            return string.Format(imageUrl, thumbName);
        }
    }
}
