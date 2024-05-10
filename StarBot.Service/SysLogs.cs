using StarBot.Entity;
using StarBot.Extension;
using StarBot.IService;
using StarBot.Repository;
using TBC.CommonLib;

namespace StarBot.Service
{
    /// <summary>
    /// 日志
    /// </summary>
    public class SysLogs : Repository<SysLog>, ISysLog
    {
        ISysConfig _sysConfig;

        public SysLogs(ISysConfig sysConfig)
        {
            _sysConfig = sysConfig;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public async Task WriteLog(string content, bool send = true)
        {
            var log = new SysLog()
            {
                Content = content,
                Time = DateTime.Now
            };
            if (!ReciverMsg.BotReady) return;
            if (send)
                await ReciverMsg.Instance.SendAdminErrorMsg(content);
            await AddAsync(log);
        }
    }
}
