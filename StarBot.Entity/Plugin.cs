using SqlSugar;

namespace StarBot.Entity
{

    /// <summary>
    /// 插件表
    /// </summary>
    [SugarTable("Plugin")]
    public class Plugin
    {
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 插件名
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        public string? Desc { get; set; }

        /// <summary>
        /// 使用方法
        /// </summary>
        public string? Usage { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "0.0.1";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string ConfPath { get; set; } = "";
        [SugarColumn(IsIgnore = true)]
        public string LogPath { get; set; } = "";
    }
}
