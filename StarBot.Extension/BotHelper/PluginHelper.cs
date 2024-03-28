using Flurl.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginServer;
using ShamrockCore;
using System.Reflection;
using System.Runtime.Loader;

namespace StarBot.Extension
{
    /// <summary>
    /// 插件帮助类
    /// </summary>
    public class PluginHelper : IDisposable
    {
        public static List<PluginHelper> Plugins { get; } = [];
        public BasePlugin? PluginInfo { get; set; }
        public bool Status { get; set; }
        public string FileName { get; set; } = "";
        private CustomAssemblyLoadContext? DLL { get; set; }

        /// <summary>
        /// 加载插件
        /// </summary>
        public static void LoadPlugins()
        {
            if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");
            var files = new DirectoryInfo("Plugins").GetFiles();
            var delConf = "Plugins/conf/del.txt";
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
                var dll = new CustomAssemblyLoadContext();
                Assembly assembly = dll.LoadFromAssemblyPath(item.FullName);
                // 获取 DLL 中的类型
                Type[] types = assembly.GetTypes();
                if (types == null) continue;
                var type = types.FirstOrDefault(x => x.Name == item.Name.Replace(".dll", ""));
                if (Activator.CreateInstance(types[2]) is not BasePlugin instance) continue;
                if (!Plugins.Exists(t => t.PluginInfo?.Name == instance.Name && t.PluginInfo.Version == instance.Version))
                {
                    Plugins.Add(new()
                    {
                        PluginInfo = instance,
                        DLL = dll,
                        Status = true,
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
            return (true, "启用成功！");
        }

        public static void DelForce()
        {
            //删除要删除的插件
            if (!Directory.Exists("Plugins/conf/")) Directory.CreateDirectory("Plugins/conf/");
            var delConf = "Plugins/conf/del.txt";
            if (!File.Exists(delConf)) File.Create(delConf);
            var delList = File.ReadAllLines(delConf).ToList();
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
                var delConf = "Plugins/conf/del.txt";
                if (!File.Exists(delConf)) File.Create(delConf);
                var plugin = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name);
                if (plugin == null) return true;
                plugin.Status = false;
                plugin.PluginInfo?.Dispose();
                plugin.DLL?.Unload();
                plugin.DLL?.Dispose();
                plugin.Dispose();
                Plugins.Remove(plugin);
                var newText = File.ReadAllLines(delConf).ToList();
                var delInfo = plugin.FileName;
                newText.Add(delInfo);
                File.WriteAllLines(delConf, newText);
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
                item.DLL?.Unload();
                item.DLL?.Dispose();
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
        /// <param name="bot">机器人对象</param>
        public static void Excute(Bot bot, bool useBot = false)
        {
            if (!useBot) return;
            foreach (var item in Plugins)
            {
                if (item.PluginInfo == null) continue;
                if (item.Status == false) continue;
                item.PluginInfo.Excute(bot);
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

    class CustomAssemblyLoadContext : AssemblyLoadContext, IDisposable
    {
        public CustomAssemblyLoadContext() : base(isCollectible: true)
        {
        }
        protected override Assembly? Load(AssemblyName name)
        {
            return null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
