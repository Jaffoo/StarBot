using IdolBot.Entity;
using IdolBot.Model;
using IdolBot.Repository;

namespace IdolBot.IService
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public interface IConfigs : IRepository<Config>
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<string> GetValue(string key);
    }
}