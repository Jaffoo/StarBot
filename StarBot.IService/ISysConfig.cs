using StarBot.Entity;
using StarBot.Model;
using StarBot.Repository;

namespace StarBot.IService
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

        Task<bool> SaveConfig(Config config);

        void ClearConfig(string key = "");
    }
}