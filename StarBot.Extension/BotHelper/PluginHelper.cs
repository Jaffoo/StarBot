using PluginServer;
using System.Reflection;
using UnifyBot.Receiver.EventReceiver;
using UnifyBot.Receiver.MessageReceiver;
using UnifyBot;
using SqlSugar;
using FluentScheduler;
using TBC.CommonLib;
using StarBot.Helper;
using StarBot.Repository;
using StarBot.Entity;

namespace UmoBot.Extension
{
    /// <summary>
    /// 插件帮助类
    /// </summary>
    public class PluginHelper() : IDisposable
    {
        private static IRepository<Plugin> _plugin { get; set; } = new Repository<Plugin>();
        private static IRepository<PluginConfig> _config { get; set; } = new Repository<PluginConfig>();
        public static List<Plugin> Plugins
        {
            get
            {
                return _plugin.GetListAsync().Result;
            }
        }
        private static readonly Dictionary<Plugin, BasePlugin> LoadedPlugins = [];

        /// <summary>
        /// 加载插件
        /// </summary>
        public async static Task LoadPlugins(Bot bot)
        {
            if (!Directory.Exists("plugins")) Directory.CreateDirectory("plugins");
            var files = new DirectoryInfo("plugins").GetFiles();
            foreach (var item in files)
            {
                if (item.Extension != ".dll") continue;
                byte[] buffurs = File.ReadAllBytes(item.FullName);
                Assembly assembly = Assembly.Load(buffurs);
                // 获取 DLL 中的类型
                Type[] types = assembly.GetTypes();
                if (types == null) continue;
                var type = types.FirstOrDefault(x => !x.Name.Contains("<"));
                if (type == null) continue;
                var instanceObj = Activator.CreateInstance(type);
                // 获取私有属性的 PropertyInfo
                var pFields = typeof(BasePlugin).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                // 检查属性信息是否为 null
                if (pFields != null)
                {
                    var pBot = pFields.FirstOrDefault(x => x.Name.Contains("_bot"));
                    pBot?.SetValue(instanceObj, bot);
                }
                using BasePlugin? instance = instanceObj as BasePlugin;
                if (instance == null) continue;
                if (instance.ServerVersion != new ServerVersion().ServerVersion) continue;
                Plugin temp;
                if (!Plugins.Exists(t => t.Name == instance.Name && t.Version == instance.Version))
                {
                    temp = new()
                    {
                        Name = instance.Name,
                        Version = instance.Version,
                        Enable = false,
                        Usage = instance.Useage,
                        Desc = instance.Desc,
                        LogPath = instance.LogPath,
                        ConfPath = instance.ConfPath,
                    };
                    var id = await _plugin.AddAsync(temp);
                    temp.Id = id;
                    instance.PluginId = id;
                }
                else
                {
                    temp = Plugins.FirstOrDefault(x => x.Name == instance.Name && x.Version == instance.Version)!;
                }
                if (!LoadedPlugins.Any(x => x.Key.Name == instance.Name && x.Key.Version == instance.Version))
                    LoadedPlugins.Add(temp, instance);
            }
        }

        /// <summary>
        /// 重载插件
        /// </summary>
        public async static Task ReLoadPlugins(Bot bot)
        {
            Plugins.Clear();
            LoadedPlugins.Clear();
            await LoadPlugins(bot);
        }

        /// <summary>
        /// 禁用插件
        /// </summary>
        public async static Task<bool> StopPlugin(long id)
        {
            var plugin = Plugins.FirstOrDefault(t => t.Id == id);
            if (plugin == null) throw new Exception("插件不存在");
            plugin.Enable = false;
            var b = await _plugin.UpdateAsync(plugin);
            if (b)
            {
                var load = LoadedPlugins.FirstOrDefault(x => x.Key.Id == id);
                load.Key.Enable = false;
                if (!load.Value.JobName.IsNullOrWhiteSpace())
                    JobManager.GetSchedule(load.Value.JobName).Disable();
            }
            return b;
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        public async static Task<bool> StartPlugin(long id)
        {
            var plugin = Plugins.FirstOrDefault(t => t.Id == id);
            if (plugin == null) throw new Exception("插件不存在");
            var list = Plugins.Where(x => x.Name == plugin.Name).ToList();
            list.ForEach(x =>
            {
                if (x.Id == id) x.Enable = true;
                else x.Enable = false;
            });

            var b = await _plugin.UpdateRangeAsync(list);
            if (b)
            {
                var load = LoadedPlugins.FirstOrDefault(x => x.Key.Id == id);
                load.Key.Enable = true;
                if (!load.Value.JobName.IsNullOrWhiteSpace())
                    JobManager.GetSchedule(load.Value.JobName).Enable();
            }
            return b;
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        public async static Task<bool> DelPlugin(long id)
        {
            try
            {
                var b = await _plugin.DeleteAsync(id);
                if (b)
                {
                    var item = LoadedPlugins.FirstOrDefault(x => x.Key.Id == id);
                    if (!item.Value.JobName.IsNullOrWhiteSpace())
                        JobManager.RemoveJob(item.Value.JobName);
                    LoadedPlugins.Remove(item.Key);
                    var file = new DirectoryInfo("plugins").GetFiles().FirstOrDefault(x => x.Name == item.Key.Name + x.Extension);
                    file?.Delete();
                    DirectoryInfo dir = new("plugins/conf/" + item.Key.Name);
                    if (dir.Exists)
                        dir.Delete(true);
                    //删除配置数据
                    var list = await _config.GetListAsync(x => x.PluginId == item.Value.PluginId);
                    await _config.DeleteAsync(list);
                }
                return b;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 调用插件
        /// </summary>
        public static async void Excute(MessageReceiver? mrb = null, EventReceiver? eb = null, string unKnow = "")
        {
            foreach (var item in LoadedPlugins)
            {
                if (item.Key == null || item.Value == null) continue;
                if (!item.Key.Enable) continue;
                try
                {
                    await item.Value.Excute(mrb, eb, unKnow);
                }
                catch (Exception e)
                {
                    UtilHelper.WriteLog(e.Message, prefix: "error");
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class ServerVersion : BasePlugin
    {
        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Desc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Useage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
