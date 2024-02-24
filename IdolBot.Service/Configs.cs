using IdolBot.Entity;
using IdolBot.Extension;
using IdolBot.Helper;
using IdolBot.IService;
using IdolBot.Repository;

namespace IdolBot.Service
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class Configs(ICacheService cacheService) : Repository<Config>, IConfigs
    {
        private readonly ICacheService _cacheService = cacheService;

        /// <summary>
        /// 获取数据列表从缓存
        /// </summary>
        /// <returns></returns>
        public async Task<List<Config>> GetListByCache()
        {
            List<Config> list = _cacheService.GetCache<List<Config>>(ConstantHelper.CONFIGSYSTEMLISTALL);
            if (list == null)
            {
                list = await GetListAsync();//dbcontext不进行跟踪，去缓存
                _cacheService.SetCache(ConstantHelper.CONFIGSYSTEMLISTALL, list, 30);
            }
            return list;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetValue(string key)
        {
            List<Config> list = await GetListByCache();
            var model = list.Where(c => c.Key == key).FirstOrDefault();
            if (model == null)
                return "";
            else
                return model.Value ?? "";
        }
    }
}
