using UnifyBot.Model;
using UnifyBot.Receiver;
using UnifyBot.Receiver.EventReceiver;
using UnifyBot.Receiver.EventReceiver.Notice;
using UnifyBot.Receiver.MessageReceiver;

namespace PluginServer
{
    /// <summary>
    /// 插件父类
    /// </summary>
    /// <param name="name">插件名</param>
    /// <param name="version">插件版本</param>
    /// <param name="desc">插件描述</param>
    public abstract class BasePlugin : IDisposable
    {
        /// <summary>
        /// qq群超管
        /// </summary>
        public string Admin { get; set; } = "";

        /// <summary>
        /// qq群普通管理员
        /// </summary>
        public List<string> Permission { get; set; } = [];

        private bool disposedValue;
        /// <summary>
        /// 插件名称(请使用英文)
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public abstract string Desc { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public abstract string Version { get; set; }

        /// <summary>
        /// 配置文件路径（xml格式）
        /// </summary>
        public static string ConfPath
        {
            get
            {
                var dir = Path.Combine(Environment.CurrentDirectory, "plugins/conf/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        /// <summary>
        /// 日志
        /// </summary>
        public static string LogPath
        {
            get
            {
                var dir = Path.Combine(Environment.CurrentDirectory, "plugins/logs/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <returns></returns>
        public virtual async Task Excute(MessageReceiver? mr = null, EventReceiver? eb = null)
        {
            try
            {
                if (mr != null)
                {
                    if(mr.MessageType==MessageType.Private)
                        await FriendMessage((PrivateReceiver)mr!);
                    if (mr.MessageType == MessageType.Group)
                        await GroupMessage((GroupReceiver)mr!);
                }
                if (eb != null)
                {
                    await EventMessage(eb);
                }
            }
            catch (Exception)
            {
                throw;
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
        public virtual async Task FriendMessage(PrivateReceiver fmr)
        {
            await Task.Delay(1);
            return;
        }

        /// <summary>
        /// 事件消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual async Task EventMessage(EventReceiver e)
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
