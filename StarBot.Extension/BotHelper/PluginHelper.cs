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
        public PluginHelper? this[string name]
        {
            get { return Plugins.FirstOrDefault(t => t.PluginInfo?.Name == name); }
        }
        public BasePlugin? PluginInfo { get; set; }
        private CustomAssemblyLoadContext? DLL { get; set; }

        /// <summary>
        /// 加载插件
        /// </summary>
        public void LoadPlugins()
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
                    var model = Plugins.FirstOrDefault(t => t.PluginInfo?.Name == instance.Name);
                    model?.DLL?.Dispose();
                    model?.PluginInfo?.Dispose();
                    model?.Dispose();
                    Plugins.Remove(model!);
                }
                Plugins.Add(new()
                {
                    PluginInfo = instance,
                    DLL = dll,
                });
            }
        }

        /// <summary>
        /// 卸载插件（不会立即卸载完，但是插件不会被执行）
        /// </summary>
        /// <param name="name"></param>
        public void UnloadPlugins(string name)
        {
            var plugin = this[name];
            if (plugin == null) return;
            plugin.PluginInfo?.Dispose();
            plugin.DLL?.Unload();
            plugin.DLL?.Dispose();
            plugin.Dispose();
            Plugins.Remove(plugin);
        }

        /// <summary>
        /// 卸载插件（不会立即卸载完，但是插件不会被执行）
        /// </summary>
        /// <param name="name"></param>
        public void UnloadPlugins()
        {
            foreach (var item in Plugins)
            {
                item.PluginInfo?.Dispose();
                item?.DLL?.Unload();
                item?.DLL?.Dispose();
                item?.Dispose();
            }
            Plugins.Clear();
        }

        /// <summary>
        /// 重载插件
        /// </summary>
        public void ReloadPlugins()
        {
            UnloadPlugins();
            LoadPlugins();
        }

        /// <summary>
        /// 调用插件
        /// </summary>
        /// <param name="msgBase"></param>
        /// <param name="eventBase"></param>
        public async Task Excute(MessageReceiverBase? msgBase = null, EventBase? eventBase = null)
        {
            foreach (var item in Plugins)
            {
                if (item.PluginInfo == null) continue;
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
