using SqlSugar;

namespace StarBot.Entity
{
    /// <summary>
    /// 用户
    /// </summary>
    [SugarTable("SysUser")]
    public class SysUser
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = "";

        /// <summary>
        /// 角色
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; } = "";

        /// <summary>
        /// vip
        /// </summary>
        public bool Vip { get; set; }=false;

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 未知字段
        /// </summary>
        public string PfUrl { get; set; } = string.Empty;

        /// <summary>
        /// 队伍logo
        /// </summary>
        public string TeamLogo { get; set; } = string.Empty;

        /// <summary>
        /// 最后一次活跃时间
        /// </summary>
        public DateTime LastActive { get; set; }
    }
}