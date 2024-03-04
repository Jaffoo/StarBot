using SqlSugar;

namespace StarBot.Entity
{
    /// <summary>
    /// 偶像信息
    /// </summary>
    [SugarTable("SysIdol")]
    public class SysIdol
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 房价id
        /// </summary>
        public string RoomId { get; set; } = "";

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; } = "";

        /// <summary>
        /// 服务器id
        /// </summary>
        public string ServerId { get; set; } = "";

        /// <summary>
        /// 队伍id
        /// </summary>
        public string TeamId { get; set; } = "";

        /// <summary>
        /// 直播房间id
        /// </summary>
        public string LiveId { get; set; } = "";

        /// <summary>
        /// 队伍
        /// </summary>
        public string Team { get; set; } = "";

        /// <summary>
        /// 团名称
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 团期数
        /// </summary>
        public string PeriodName { get; set; } = "";

        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin { get; set; } = "";

        /// <summary>
        /// 频道id
        /// </summary>
        public string ChannelId { get; set; } = "";
    }
}