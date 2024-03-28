using ShamrockCore;
using ShamrockCore.Receiver;
using ShamrockCore.Receiver.Receivers;
using System.Reactive.Linq;

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
        public string ConfPath
        {
            get
            {
                var dir = Path.Combine(Environment.CurrentDirectory, "Plugins/conf/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return $"{dir + Name}.xml";
            }
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="bot">机器人对象</param>
        /// <returns></returns>
        public virtual void Excute(Bot bot)
        {
            if (bot != null)
            {
                bot.MessageReceived.OfType<MessageReceiverBase>().Subscribe(async mrb =>
                {
                    await BaseMessage(mrb);
                });
                bot.MessageReceived.OfType<GroupReceiver>().Subscribe(async gr =>
                {
                    await GroupMessage(gr);
                });
                bot.MessageReceived.OfType<FriendReceiver>().Subscribe(async fr =>
                {
                    await FriendMessage(fr);
                });
                bot.EventReceived.OfType<EventBase>().Subscribe(async e =>
                {
                    await EventMessage(e);
                });
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
