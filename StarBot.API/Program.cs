using StarBot.Helper;
using StarBot.Extension;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using FluentScheduler;
using StarBot.Timer;
using Newtonsoft.Json.Serialization;
using StarBot.Repository;
using System.Reflection;

namespace StarBot.Api;

public class Program
{
    /// <summary>
    /// 初始化
    /// </summary>

    [STAThread]
    public static void Main() => InitAPI().Run();

    /// <summary>
    /// 初始化api
    /// </summary>
    public static WebApplication InitAPI()
    {
        var builder = WebApplication.CreateBuilder();

        //设置基础配置
        ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
        //注入数据
        var projectName = ConfigHelper.GetConfiguration("NameSpace");
        builder.Services.AddDataService(projectName + ".Service");

        //同步数据库结构
        DBFactory.InitTable();

        builder.Services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;//解决后端传到前端全大写
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);//解决后端返回数据中文被编码
            }).AddNewtonsoftJson(options =>
            {
                //json序列化设置
                //json序列化设置默认首字母小写驼峰命名
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

        //文件上传大小
        builder.Services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = int.MaxValue;
            options.MemoryBufferThreshold = int.MaxValue;
        });
        //缓存
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

        var app = builder.Build();

        //启动定时任务
        JobManager.Initialize(new FluentSchedulerFactory());

        //获取依赖注入的服务
        ConfigHelper.ServiceProvider = app.Services;

        //自定义异常捕获中间件
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseStaticFiles(new StaticFileOptions
        {
            //设置不限制content-type
            ServeUnknownFileTypes = true
        });
        //跨域请求设置，此方法允许所有接口跨域
        app.UseCors(builder => builder
            //允许任何来源
            .AllowAnyOrigin()
             //所有请求方法
             .AllowAnyMethod()
             //所有请求头
             .AllowAnyHeader());

        app.UseCors(option => option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        //将请求与端点匹配，匹配路由
        app.UseRouting();

        app.MapControllers();

        return app;
    }
}