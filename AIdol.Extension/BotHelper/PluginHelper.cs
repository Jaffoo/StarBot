using PluginServer;
using ShamrockCore.Receiver;
using System.Reflection;
using System.Runtime.Loader;

namespace AIdol.Extension
{
    /// <summary>
    /// 插件帮助类
    /// </summary>
    public class PluginHelper : IDisposable
    {
        public PluginHelper()
        {
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
        }
        public PluginHelper? this[string name]
        {
            get { return _plugins.FirstOrDefault(t => t.Name == name); }
        }
        private static readonly string _path = Environment.CurrentDirectory + "/Plugin";
        public List<PluginHelper> Plugins
        {
            get
            {
                return _plugins;
            }
        }
        public readonly List<PluginHelper> _plugins = [];
        private string? Name { get; set; }
        private string? Description { get; set; }
        private BasePlugin? Plugin { get; set; }
        private CustomAssemblyLoadContext? DLL { get; set; }

        /// <summary>
        /// 加载插件
        /// </summary>
        public void LoadPlugins()
        {
            var files = new DirectoryInfo(_path).GetFiles();
            foreach (var item in files)
            {
                var dll = new CustomAssemblyLoadContext();
                var assembly = dll.LoadFromAssemblyPath(item.FullName);
                // 获取 DLL 中的类型
                Type[] types = assembly.GetTypes();
                // 创建类型的实例
                var type = types.FirstOrDefault(t => t.BaseType!.Name == "Object");
                if (type == null) continue;
                if (Activator.CreateInstance(type) is not BasePlugin instance) continue;
                if (_plugins.Exists(t => t.Name == instance.Name))
                {
                    var model = _plugins.FirstOrDefault(t => t.Name == instance.Name);
                    model?.DLL?.Dispose();
                    model?.Plugin?.Dispose();
                    model?.Dispose();
                    _plugins.Remove(model!);
                }
                _plugins.Add(new()
                {
                    Name = instance.Name,
                    Description = instance.Desc,
                    Plugin = instance,
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
            plugin.Plugin?.Dispose();
            plugin.DLL?.Unload();
            plugin.DLL?.Dispose();
            plugin.Dispose();
            _plugins.Remove(plugin);
        }

        /// <summary>
        /// 卸载插件（不会立即卸载完，但是插件不会被执行）
        /// </summary>
        /// <param name="name"></param>
        public void UnloadPlugins()
        {
            foreach (var item in _plugins)
            {
                item.Plugin?.Dispose();
                item?.DLL?.Unload();
                item?.DLL?.Dispose();
                item?.Dispose();
            }
            _plugins.Clear();
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
            foreach (var item in _plugins)
            {
                if (item.Plugin == null) continue;
                await item.Plugin.Excute(msgBase, eventBase);
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
