using StarBot.Entity;
using StarBot.Extension;
using StarBot.IService;
using StarBot.Model;
using StarBot.Repository;
using Newtonsoft.Json.Linq;
using TBC.CommonLib;

namespace StarBot.Service
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

        public async Task<bool> SaveConfig(Config config)
        {
            var whriteList = new List<string>() { "MsgTypeAll" };
            var updates = new List<SysConfig>();
            var jobj = JObject.FromObject(config)!;
            foreach (var item in jobj)
            {
                var pModel = await GetModelAsync(t => t.Key == item.Key && t.DataType.Contains("class"));
                if (pModel == null) continue;
                var children = await GetListAsync(t => t.Pid == pModel.Id);
                var subItem = JObject.Parse(item.Value!.ToString());
                foreach (var child in children)
                {
                    if (whriteList.Contains(child.Key)) continue;
                    var newVal = subItem[child.Key]!.ToString();
                    if (newVal == "False" || newVal == "True") newVal = newVal.ToLower();
                    if (newVal == "[]") newVal = "";
                    if (newVal == child.Value) continue;
                    child.Value = newVal;
                    ClearConfig(pModel.Key);
                    updates.Add(child);
                }
            }
            if (updates.Count == 0) return true;
            return await UpdateRangeAsync(updates);
        }

        public async Task<Config> GetConfig()
        {
            var config = new Config();
            //EnableModule
            var enableModule = cache.GetCache<EnableModule>("EnableModule");
            enableModule ??= await SetCache<EnableModule>(6, "EnableModule");
            config.EnableModule = enableModule;

            //Bot
            var bot = cache.GetCache<Bot>("Bot");
            bot ??= await SetCache<Bot>(1, "Bot");
            config.Bot = bot;

            //qq
            var qq = cache.GetCache<QQ>("QQ");
            qq ??= await SetCache<QQ>(8, "QQ");
            config.QQ = qq;

            //WB
            var wb = cache.GetCache<WB>("WB");
            wb ??= await SetCache<WB>(9, "WB");
            config.WB = wb;

            //BZ
            var bz = cache.GetCache<BZ>("BZ");
            bz ??= await SetCache<BZ>(10, "BZ");
            config.BZ = bz;

            //KD
            var kd = cache.GetCache<KD>("KD");
            kd ??= await SetCache<KD>(11, "KD");
            config.KD = kd;

            //XHS
            var xhs = cache.GetCache<XHS>("XHS");
            xhs ??= await SetCache<XHS>(12, "XHS");
            config.XHS = xhs;

            //DY
            var dy = cache.GetCache<DY>("DY");
            dy ??= await SetCache<DY>(13, "DY");
            config.DY = dy;

            //BD
            var bd = cache.GetCache<BD>("BD");
            bd ??= await SetCache<BD>(14, "BD");
            config.BD = bd;

            return config;
        }

        private async Task<T> SetCache<T>(int pid, string key)
        {
            var list = await GetListAsync(t => t.Pid == pid);
            JObject obj = [];
            foreach (var item in list)
            {
                if (item.DataType.Contains("bool"))
                    obj.Add(item.Key, item.Value.ToBool(false));
                else if (item.DataType.Contains("list"))
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                        obj.Add(item.Key, new JArray());
                    else
                        obj.Add(item.Key, JArray.Parse(item.Value));
                }
                else if (item.DataType.Contains("int"))
                    obj.Add(item.Key, item.Value.ToInt(0));
                else obj.Add(item.Key, item.Value);
            }
            var val = obj.ToObject<T>()!;
            cache.SetCache(key, val);
            return val;
        }

        public void ClearConfig(string key = "")
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                cache.RemoveCache(key);
                return;
            }
            cache.RemoveCache("EnableModule");
            cache.RemoveCache("Bot");
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
