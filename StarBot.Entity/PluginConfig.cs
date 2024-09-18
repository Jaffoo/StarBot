using SqlSugar;

namespace StarBot.Entity;

/// <summary>
/// 插件配置类
/// </summary>
[SugarTable("PluginConfig")]
public class PluginConfig
{
    /// <summary>
    /// id
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 插件id
    /// </summary>
    public int PluginId { get; set; }

    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; } = "";
}
