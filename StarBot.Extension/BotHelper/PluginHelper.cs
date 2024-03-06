using PluginServer;
using ShamrockCore.Receiver;
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
        public PluginHelper()
        {
            if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");
        }
        public BasePlugin? PluginInfo { get; set; }
        public bool Status { get; set; }
        private string Path { get; set; } = "";
        private CustomAssemblyLoadContext? DLL { get; set; }

        /// <summary>
        /// 加载插件
        /// </summary>
        public static void LoadPlugins()
        {
            var files = new DirectoryInfo("Plugins").GetFiles();
            foreach (var item in files)
            {
                var dll = new CustomAssemblyLoadContext();
                var assembly = dll.LoadFromAssemblyPath(item.FullName);
                // 获取 DLL 中的类型
                Type[] types = assembly.GetTypes();
                // 创建类型的实例
                var type = types.FirstOrDefault();
                if (type == null) continue;
                if (Activator.CreateInstance(type) is not BasePlugin instance) continue;
                if (Plugins.Exists(t => t.PluginInfo?.Name == instance.Name))
                {
                    var model = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == instance.Name && t.PluginInfo.Version != instance.Version);
                    model?.DLL?.Dispose();
                    model?.PluginInfo?.Dispose();
                    model?.Dispose();
                    Plugins.Remove(model!);
                }
                Plugins.Add(new()
                {
                    PluginInfo = instance,
                    DLL = dll,
                    Status = true,
                    Path = item.FullName
                });
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
            plugin.Status = false;
            return (true, "启用成功！");
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="name"></param>
        public static bool DelPlugin(string name)
        {
            var plugin = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name);
            if (plugin == null) return true;
            plugin.PluginInfo?.Dispose();
            plugin.DLL?.Unload();
            plugin.DLL?.Dispose();
            plugin.Dispose();
            File.Delete(plugin.Path);
            return true;
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
        /// <param name="msgBase"></param>
        /// <param name="eventBase"></param>
        public static async Task Excute(MessageReceiverBase? msgBase = null, EventBase? eventBase = null)
        {
            foreach (var item in Plugins)
            {
                if (item.PluginInfo == null) continue;
                if (item.Status == false) continue;
                await item.PluginInfo.Excute(msgBase, eventBase);
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
