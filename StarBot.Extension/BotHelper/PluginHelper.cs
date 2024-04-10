using Microsoft.Extensions.DependencyInjection;
using PluginServer;
using StarBot.IService;
using StarBot.Model;
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
        private static readonly string delConf = "Plugins/del.txt";

        private static readonly string startConf = "Plugins/start.txt";

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
            if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");
            var files = new DirectoryInfo("Plugins").GetFiles();
            List<FileInfo> delInfo = [];
            if (File.Exists(delConf))
            {
                var list = File.ReadLines(delConf);
                foreach (var filestr in list)
                {
                    delInfo.Add(new(filestr));
                }
            }
            foreach (var item in files)
            {
                if (delInfo != null)
                {
                    if (delInfo.Any(x => x.Name == item.Name)) continue;
                }
                if (item.Extension != ".dll") continue;
                Assembly assembly = Assembly.LoadFrom(item.FullName);
                // 获取 DLL 中的类型
                Type[] types = assembly.GetTypes();
                if (types == null) continue;
                var type = types.FirstOrDefault(x => x.Name == item.Name.Replace(".dll", ""));
                if (type == null) continue;
                if (Activator.CreateInstance(type) is not BasePlugin instance) continue;
                instance.Permission = Config.QQ.Permission.ToStrList();
                instance.Admin = Config.QQ.Admin;
                List<string> startList = [];
                if (File.Exists(startConf))
                {
                    startList = File.ReadLines(startConf).ToList();
                }
                if (!Plugins.Exists(t => t.PluginInfo?.Name == instance.Name && t.PluginInfo.Version == instance.Version))
                {
                    var status = false;
                    if (startList.Any(t => t == item.FullName))
                        status = true;
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
            var plugin = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name);
            if (plugin == null) return (false, "插件不存在！");
            plugin.Status = false;
            List<string> startList = [];
            if (File.Exists(startConf))
            {
                startList = File.ReadLines(startConf).ToList();
            }
            if (startList.Any(t => t == plugin.FileName))
            {
                startList.Remove(plugin.FileName);
                File.WriteAllLines(startConf, startList);
            }
            return (true, "禁用成功！");
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="name"></param>
        public static (bool b, string msg) StartPlugin(string name)
        {
            var plugin = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name);
            if (plugin == null) return (false, "插件不存在！");
            plugin.Status = true;
            List<string> startList = [];
            if (File.Exists(startConf))
            {
                startList = File.ReadLines(startConf).ToList();
            }
            if (!startList.Any(t => t == plugin.FileName))
            {
                startList.Add(plugin.FileName);
                File.WriteAllLines(startConf, startList);
            }
            return (true, "启用成功！");
        }

        public static void DelForce()
        {
            //删除要删除的插件
            if (!Directory.Exists("Plugins/conf/")) Directory.CreateDirectory("Plugins/conf/");
            if (!File.Exists(delConf)) File.Create(delConf);
            List<string> delList = [];
            if (File.Exists(delConf))
            {
                delList = File.ReadLines(delConf).ToList();
            }
            if (delList.Count > 0)
            {
                List<string> del = [];
                foreach (var item in delList)
                {
                    File.Delete(item);
                    del.Add(item);
                }
                foreach (var item in del)
                {
                    delList.Remove(item);
                }
                File.WriteAllLines(delConf, delList);
            }
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="name"></param>
        public static bool DelPlugin(string name)
        {
            try
            {
                if (!Directory.Exists("Plugins/conf/")) Directory.CreateDirectory("Plugins/conf/");
                if (!File.Exists(delConf)) File.Create(delConf);
                var plugin = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name);
                if (plugin == null) return true;
                plugin.Status = false;
                plugin.PluginInfo?.Dispose();
                plugin.Dispose();
                Plugins.Remove(plugin);
                var newText = File.ReadAllLines(delConf).ToList();
                var delInfo = plugin.FileName;
                newText.Add(delInfo);
                File.WriteAllLines(delConf, newText);
                List<string> startList = [];
                if (File.Exists(startConf))
                {
                    startList = File.ReadLines(startConf).ToList();
                }
                if (startList.Any(t => t == plugin.FileName))
                {
                    startList.Remove(plugin.FileName);
                    File.WriteAllLines(startConf, startList);
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
