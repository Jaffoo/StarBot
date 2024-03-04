using StarBot.Entity;
using StarBot.Repository;

namespace StarBot.IService
{
    /// <summary>
    /// 日志
    /// </summary>
    public interface ISysLog : IRepository<SysLog>
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        Task WriteLog(string content);
    }
}