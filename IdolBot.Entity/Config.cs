using SqlSugar;

namespace IdolBot.Entity
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [SugarTable("sys_config")]
    public class Config
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// 值
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Desc { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}