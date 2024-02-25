using SqlSugar;

namespace IdolBot.Entity
{
    /// <summary>
    /// 缓存
    /// </summary>
    [SugarTable("SysCache")]
    public class SysCache
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Content { get; set; } = "";
        
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 类型1-微博
        /// </summary>
        public int Type { get; set; }
    }
}