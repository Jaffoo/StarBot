using SqlSugar;

namespace AIdol.Entity
{
    /// <summary>
    /// 日志
    /// </summary>
    [SugarTable("SysLog")]
    public class SysLog
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
    }
}