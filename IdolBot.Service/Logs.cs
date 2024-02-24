using IdolBot.Entity;
using IdolBot.IService;
using IdolBot.Model;
using IdolBot.Repository;

namespace IdolBot.Service
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Logs : Repository<Log>, ILogs
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="logLevel">等级</param>
        /// <param name="url">路由地址</param>
        /// <returns></returns>
        public async Task WriteLog(string title, string content, Enums.LogLevel logLevel, string? url = null)
        {
            var log = new Log()
            {
                Title = title,
                Info = content,
                Level = Enum.GetName(typeof(Enums.LogLevel), logLevel)!,
                CreateDate = DateTime.Now,
                Url = url
            };
            await AddAsync(log);
        }
    }
}
