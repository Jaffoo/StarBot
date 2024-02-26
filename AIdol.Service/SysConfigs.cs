using AIdol.Entity;
using AIdol.Extension;
using AIdol.IService;
using AIdol.Model;
using AIdol.Repository;
using Newtonsoft.Json.Linq;
using TBC.CommonLib;

namespace AIdol.Service
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SysConfigs(ICacheService _cache) : Repository<SysConfig>, ISysConfig
    {
        ICacheService cache = _cache;
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetValue(string key, int pid = 0)
        {
            var model = new SysConfig();
            if (pid > 0)
                model = await GetModelAsync(t => t.Pid == pid && t.Key == key);
            else
                model = await GetModelAsync(t => t.Key == key);
            return model.Value;
        }

        public async Task<Config> GetConfig()
        {
            var config = new Config();
            //enable
            var enableModule = cache.GetCache<EnableModule>("EnableModule");
            if (enableModule == null) await SetCache(6, "EnableModule");
            else config.EnableModule = enableModule;

            //shamrock
            var shamrock = cache.GetCache<Shamrock>("OpenShamrock");
            if (enableModule == null) await SetCache(1, "OpenShamrock");
            else config.Shamrock = shamrock;

            //qq
            var qq = cache.GetCache<QQ>("QQ");
            if (enableModule == null) await SetCache(8, "QQ");
            else config.QQ = qq;

            //WB
            var wb = cache.GetCache<WB>("WB");
            if (enableModule == null) await SetCache(9, "WB");
            else config.WB = wb;

            //BZ
            var bz = cache.GetCache<BZ>("BZ");
            if (enableModule == null) await SetCache(12, "BZ");
            else config.BZ = bz;

            //KD
            var kd = cache.GetCache<KD>("KD");
            if (enableModule == null) await SetCache(11, "KD");
            else config.KD = kd;

            //XHS
            var xhs = cache.GetCache<XHS>("XHS");
            if (enableModule == null) await SetCache(12, "XHS");
            else config.XHS = xhs;

            //DY
            var dy = cache.GetCache<DY>("DY");
            if (enableModule == null) await SetCache(13, "DY");
            else config.DY = dy;

            //BD
            var bd = cache.GetCache<BD>("BD");
            if (enableModule == null) await SetCache(14, "BD");
            else config.BD = bd;

            return config;
        }

        private async Task SetCache(int pid, string key)
        {
            var list = await GetListAsync(t => t.Pid == pid);
            JObject obj = [];
            foreach (var item in list)
            {
                if (item.Value.Contains("bool"))
                    obj.Add(item.Key, item.Value.ToBool());
                else if (item.Value.Contains("list"))
                    obj.Add(item.Key, JArray.Parse(item.Value));
                else if (item.Value.Contains("int"))
                    obj.Add(item.Key, item.Value.ToInt());
                else obj.Add(item.Key, item.Value);
            }
            cache.SetCache(key, JObject.FromObject(obj));
        }

        public void ClearConfig(string key = "")
        {
            if (!string.IsNullOrWhiteSpace(key))
                cache.RemoveCache(key);
            else
            {
                cache.RemoveCache("EnableModule");
                cache.RemoveCache("Shamrock");
                cache.RemoveCache("QQ");
                cache.RemoveCache("WB");
                cache.RemoveCache("XHS");
                cache.RemoveCache("BD");
                cache.RemoveCache("BZ");
                cache.RemoveCache("DY");
                cache.RemoveCache("KD");
            }
        }
    }
}
