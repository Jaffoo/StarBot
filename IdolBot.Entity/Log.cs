using SqlSugar;

namespace IdolBot.Entity
{
    /// <summary>
    /// 日志
    /// </summary>
    [SugarTable("sys_log")]
    public class Log
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "";
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Info { get; set; } = "";
        
        /// <summary>
        /// 日志级别(DEBUG,INFO,WARNING,ERROR,CRITICAL)
        /// </summary>
        public string Level { get; set; } = "";
        
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        public string? Url { get; set; }
    }
}