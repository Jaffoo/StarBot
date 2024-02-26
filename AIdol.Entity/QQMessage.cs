using SqlSugar;

namespace AIdol.Entity
{
    /// <summary>
    /// qq消息
    /// </summary>
    [SugarTable("QQMessage")]
    public class QQMessage
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 发送人id
        /// </summary>
        public string SenderId { get; set; } = string.Empty;

        /// <summary>
        /// 发送人
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// 接收人/群id
        /// </summary>
        public string ReciverId { get; set; } = string.Empty;

        /// <summary>
        /// 接收人/群
        /// </summary>
        public string ReciverName { get; set; } = string.Empty;

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string? Url {  get; set; }
    }
}
