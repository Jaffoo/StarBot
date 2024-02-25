using IdolBot.Entity;
using IdolBot.Model;
using IdolBot.Repository;

namespace IdolBot.IService
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public interface ISysConfig : IRepository<SysConfig>
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        Task<string> GetValue(string key, int pid = 0);

        Task<Config> GetConfig();

        void ClearConfig(string key = "");
    }
}