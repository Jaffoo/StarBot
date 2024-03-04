namespace StarBot.Model
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public class MessageSend
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// 发送人
        /// </summary>
        public List<int> Senders { get; set; } = [];
    }
}
