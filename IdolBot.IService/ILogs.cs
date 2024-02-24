
using IdolBot.Entity;
using IdolBot.Model;
using IdolBot.Repository;

namespace IdolBot.IService
{
    /// <summary>
    /// 日志
    /// </summary>
    public interface ILogs : IRepository<Log>
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="logLevel">等级</param>
        /// <param name="url">路由</param>
        /// <returns></returns>
        Task WriteLog(string title, string content, Enums.LogLevel logLevel, string? url = null);
    }
}