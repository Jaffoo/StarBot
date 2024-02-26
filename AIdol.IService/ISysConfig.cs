using AIdol.Entity;
using AIdol.Model;
using AIdol.Repository;

namespace AIdol.IService
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