using Helper;
using ShamrockCore.Data.Model;
using ShamrockCore.Receiver;
using ShamrockCore.Receiver.Receivers;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Loader;

namespace IdolBot.Helper
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
        private readonly List<PluginHelper> _plugins = [];
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

    /// <summary>
    /// 插件父类
    /// </summary>
    /// <param name="name">插件名</param>
    /// <param name="version">插件版本</param>
    /// <param name="desc">插件描述</param>
    public class BasePlugin(string name, string version, string desc) : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Desc { get; set; } = desc;

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = version;

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="msgBase">消息基类</param>
        /// <param name="eventBase">事件基类</param>
        /// <returns></returns>
        public virtual async Task Excute(MessageReceiverBase? msgBase = null, EventBase? eventBase = null)
        {
            if (msgBase != null)
            {
                switch (msgBase.Type)
                {
                    case PostMessageType.Friend:
                        await FriendMessage((FriendReceiver)msgBase!);
                        break;
                    case PostMessageType.Group:
                        await GroupMessage((GroupReceiver)msgBase!);
                        break;
                    default:
                        await BaseMessage(msgBase);
                        break;
                }
            }
            if (eventBase != null)
            {
                await EventMessage(eventBase);
            }
        }

        /// <summary>
        /// 群消息
        /// </summary>
        /// <param name="gmr"></param>
        /// <returns></returns>
        public virtual async Task GroupMessage(GroupReceiver gmr)
        {
            await Task.Delay(1);
            return;
        }

        /// <summary>
        /// 好友消息
        /// </summary>
        /// <param name="fmr"></param>
        /// <returns></returns>
        public virtual async Task FriendMessage(FriendReceiver fmr)
        {
            await Task.Delay(1);
            return;
        }

        /// <summary>
        /// 所有消息
        /// </summary>
        /// <param name="fmr"></param>
        /// <returns></returns>
        public virtual async Task BaseMessage(MessageReceiverBase mrb)
        {
            await Task.Delay(1);
            return;
        }

        /// <summary>
        /// 事件消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual async Task EventMessage(EventBase e)
        {
            await Task.Delay(1);
            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BasePlugin()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
