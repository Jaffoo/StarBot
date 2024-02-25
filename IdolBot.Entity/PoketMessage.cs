using SqlSugar;

namespace IdolBot.Entity
{
    /// <summary>
    /// 口袋消息
    /// </summary>
    [SugarTable("PoketMessage")]
    public class PoketMessage
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 发送人
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// 频道id
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// 频道名
        /// </summary>
        public int ChannelName { get; set; }

        /// <summary>
        /// 发送者在频道的角色
        /// </summary>
        public int ChannelRole { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type { get; set; } = "text";

        /// <summary>
        /// 全部信息
        /// </summary>
        public string FullInfo { get; set; } = "";
    }
}