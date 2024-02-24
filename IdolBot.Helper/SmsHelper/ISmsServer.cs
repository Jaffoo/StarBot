namespace IdolBot.Helper
{
    /// <summary>
    /// 短信服务
    /// </summary>
    public interface ISmsServer
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="number">发送号码</param>
        /// <param name="content">短信内容</param>
        /// <param name="error">返回错误</param>
        /// <returns></returns>
        bool SendMsg(string number, string content, ref string error);

        /// <summary>
        /// 发送短链接
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        (string ShortUrl, string ErrorMsg) CreateShortUrl(string originalUrl);
    }
}
