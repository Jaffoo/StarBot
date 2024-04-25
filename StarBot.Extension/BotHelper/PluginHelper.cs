using Microsoft.Extensions.DependencyInjection;
using PluginServer;
using StarBot.IService;
using StarBot.Model;
using System;
using System.Data;
using System.Reflection;
using TBC.CommonLib;
using UnifyBot.Receiver.EventReceiver;
using UnifyBot.Receiver.MessageReceiver;

namespace StarBot.Extension
{
    /// <summary>
    /// 插件帮助类
    /// </summary>
    public class PluginHelper : IDisposable
    {
        static ISysConfig _sysConfig
        {
            get
            {
                var factory = DataService.BuildServiceProvider();
                return factory.GetService<ISysConfig>()!;
            }
        }
        static ISysLog _sysLog
        {
            get
            {
                var factory = DataService.BuildServiceProvider();
                return factory.GetService<ISysLog>()!;
            }
        }
        public static List<PluginHelper> Plugins { get; } = [];
        public BasePlugin? PluginInfo { get; set; }
        public bool Status { get; set; }
        public string FileName { get; set; } = "";
        private static List<string> DefalultStart
        {
            get
            {
                if (!Directory.Exists("plugins")) Directory.CreateDirectory("plugins");
                var path = "plugins/start.txt";
                if (!File.Exists(path))
                {
                    File.Create(path);
                    return [];
                }
                else
                {
                    var list = File.ReadAllLines(path).ToList();
                    return list;
                }
            }
        }

        private static Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        public static void LoadPlugins()
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
                if (Activator.CreateInstance(type) is not BasePlugin instance) continue;
                instance.Permission = Config.QQ.Permission.ToStrList();
                instance.Admin = Config.QQ.Admin;
                if (!Plugins.Exists(t => t.PluginInfo?.Name == instance.Name && t.PluginInfo.Version == instance.Version))
                {
                    var status = false;
                    status = DefalultStart.Any(x => x == instance.Name + ":" + instance.Version);
                    Plugins.Add(new()
                    {
                        PluginInfo = instance,
                        Status = status,
                        FileName = item.FullName,
                    });
                }
            }
        }

        /// <summary>
        /// 禁用插件
        /// </summary>
        /// <param name="name"></param>
        public static (bool b, string msg) StopPlugin(string name)
        {
            var plugins = Plugins.Where(t => t.PluginInfo?.Name == name).ToList();
            if (plugins == null) return (false, "插件不存在！");
            plugins.ForEach(x => x.Status = false);
            if (DefalultStart.Any(x => x.Contains(name)))
            {
                var startList = new List<string>();
                startList.AddRange(DefalultStart);
                var list = startList.Where(x => x.Contains(name)).ToList();
                foreach (var item in list)
                    startList.Remove(item);
                UpdateStart(startList);
            }
            return (true, "禁用成功！");
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="name"></param>
        public static (bool b, string msg) StartPlugin(string name, string version)
        {
            var plugin = Plugins.FirstOrDefault(t => t.PluginInfo!.Name == name && t.PluginInfo.Version == version);
            if (plugin == null) return (false, "插件不存在！");
            plugin.Status = true;
            var others = Plugins.Where(x => x.PluginInfo!.Name == name && x.PluginInfo.Version != version);
            if (others != null && others.Any())
                foreach (var item in others)
                    item.Status = false;

            var startList = new List<string>();
            startList.AddRange(DefalultStart);
            if (DefalultStart.Any(x => x.Contains(name)))
            {
                var list = startList.Where(x => x.Contains(name)).ToList();
                foreach (var item in list)
                    startList.Remove(item);
            }
            startList.Add(name + ":" + version);
            UpdateStart(startList);
            return (true, "启用成功！");
        }

        private static void UpdateStart(List<string> startList)
        {
            File.WriteAllLines("plugins/start.txt", startList);
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="name"></param>
        public static bool DelPlugin(string name)
        {
            try
            {
                var plugin = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name);
                if (plugin == null) return true;
                plugin.Status = false;
                plugin.PluginInfo?.Dispose();
                plugin.Dispose();
                Plugins.Remove(plugin);
                File.Delete(plugin.FileName);

                var startList = new List<string>();
                startList.AddRange(DefalultStart);
                if (DefalultStart.Any(x => x.Contains(name)))
                {
                    var list = startList.Where(x => x.Contains(name)).ToList();
                    foreach (var item in list)
                        startList.Remove(item);
                    UpdateStart(startList);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="name"></param>
        private static void UnloadPlugins()
        {
            foreach (var item in Plugins)
            {
                item.PluginInfo?.Dispose();
                item.Dispose();
                item.Status = false;
            }
            Plugins.Clear();
        }

        /// <summary>
        /// 重载插件
        /// </summary>
        public static void ReloadPlugins()
        {
            UnloadPlugins();
            LoadPlugins();
        }

        /// <summary>
        /// 调用插件
        /// </summary>
        public static async void Excute(MessageReceiver? mrb = null, EventReceiver? eb = null)
        {
            foreach (var item in Plugins)
            {
                if (item.PluginInfo == null) continue;
                if (!item.Status) continue;
                try
                {
                    await item.PluginInfo.Excute(mrb, eb);
                }
                catch (Exception e)
                {
                    await _sysLog.WriteLog($"插件【{item.PluginInfo.Name + "-" + item.PluginInfo.Version}】抛出异常：" + e.Message);
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
}
