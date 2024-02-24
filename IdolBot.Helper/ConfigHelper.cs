using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdolBot.Helper
{
    public static class ConfigHelper
    {
        /// <summary>
        /// 设置/获取系统的服务提供器
        /// </summary>
        public static IServiceProvider? ServiceProvider { get; set; }

        /// <summary>
        /// 获取配置
        /// </summary>
        public static IConfiguration? Configuration { get; set; }

        /// <summary>
        /// 获取应用程序路径
        /// </summary>
        public static string? ContentRootPath { get; set; }

        /// <summary>
        /// 获取静态资源根路径
        /// </summary>
        public static string? WebRootPath { get; set; }

        /// <summary>
        /// 设置基础信息
        /// </summary>
        public static void SetConfig(IConfiguration configuration, string contentRootPath, string webRootPath)
        {
            Configuration = configuration;
            ContentRootPath = contentRootPath ?? "";
            WebRootPath = webRootPath ?? "";
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="T">表</typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return ServiceProvider!.GetService<T>()!;
        }

        /// <summary>
        /// 获取配置文件值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string key)
        {
            if (Configuration == null) return "";
            return Configuration[key] ?? "";
        }
    }
}
