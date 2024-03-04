using Newtonsoft.Json;

namespace StarBot.Model
{
    /// <summary>
    /// 请求返回统一格式
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiResult()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="success">是否成功</param>
        /// <param name="code">状态码</param>
        /// <param name="list">数据</param>
        /// <param name="recordCount">总记录数</param>
        public ApiResult(bool success, string msg, int code, dynamic list, int recordCount)
        {
            Success = success;
            Msg = msg;
            Code = code;
            Data = list;
            Count = recordCount;
        }

        /// <summary>
        /// 构造函数，成功返回列表
        /// </summary>
        /// <param name="list">数据</param>
        /// <param name="recordCount">总记录数</param>
        public ApiResult(dynamic list, int recordCount)
        {
            Success = true;
            Data = list;
            Count = recordCount;
        }

        /// <summary>
        /// 构造函数，操作是否成功
        /// </summary>
        /// <param name="success">成功</param>
        /// <param name="code">状态码</param>
        /// <param name="msg">消息</param>
        public ApiResult(bool success, int code, string msg)
        {
            this.Success = success;
            this.Code = code;
            this.Msg = msg;
        }

        /// <summary>
        /// 构造函数，操作是否成功
        /// </summary>
        /// <param name="success">成功</param>
        /// <param name="msg">消息</param>
        public ApiResult(bool success, string msg)
        {
            this.Success = success;
            if (success)
            {
                Code = 200;
            }
            else
            {
                Code = 500;
            }
            this.Msg = msg;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { set; get; } = 200;

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Count { set; get; } = 0;

        /// <summary>
        /// 数据
        /// </summary>
        public dynamic? Data { set; get; } = null;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { set; get; } = "";

        /// <summary>
        /// 序列化为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
