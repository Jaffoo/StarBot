using AIdol.Entity;
using AIdol.IService;
using AIdol.Repository;

namespace AIdol.Service
{
    /// <summary>
    /// 日志
    /// </summary>
    public class SysLogs : Repository<SysLog>, ISysLog
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public async Task WriteLog(string content)
        {
            var log = new SysLog()
            {
                Content = content,
                Time = DateTime.Now
            };
            await AddAsync(log);
        }
    }
}