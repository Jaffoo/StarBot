using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StarBot.Helper;
using System.Reflection;

namespace StarBot.Extension
{
    /// <summary>
    /// 系统数据服务
    /// </summary>
    public static class DataService
    {
        /// <summary>
        /// 注入数据
        /// </summary>
        /// <param name="services">容器</param>
        /// <param name="assemblyName">实现类所属程序集</param>
        public static IServiceCollection AddDataService(this IServiceCollection services, string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                //载入dll程序集
                Assembly assembly = Assembly.Load(assemblyName);
                //获取所有类型列表
                List<Type> ts = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType).ToList();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    //获取接口
                    var interfaceType = item.GetInterfaces();
                    if (interfaceType.Length == 1)
                    {
                        services.AddTransient(interfaceType[0], item);
                    }
                    if (interfaceType.Length > 1)
                    {
                        //依赖注入，生命周期：每次都获取一个新的实例
                        services.AddTransient(interfaceType[1], item);
                    }
                }
            }
            return services;
        }


        /// <summary>
        /// 构造依赖注入容器
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider BuildServiceProvider()
        {

            var builder = WebApplication.CreateBuilder();

            //设置基础配置
            ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);

            #region 注入数据
            builder.Services.AddDataService(ConfigHelper.GetConfiguration("NameSpace") + ".Service");
            #endregion

            //启用dotnet本地缓存服务
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
            return builder.Services.BuildServiceProvider(); //构建服务提供程序
        }
    }
}
