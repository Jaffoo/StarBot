using StarBot.Helper;
using StarBot.Extension;
using Microsoft.AspNetCore.Http.Features;
using StarBot.Repository;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using FluentScheduler;
using StarBot.Timer;

var builder = WebApplication.CreateBuilder(args);

//设置基础配置
ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
//注入数据
var projectName = ConfigHelper.GetConfiguration("NameSpace");
builder.Services.AddDataService(projectName + ".Service");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;//解决后端传到前端全大写
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);//解决后端返回数据中文被编码
    }).AddNewtonsoftJson(options =>
    {
        //json序列化设置
        //json序列化设置默认首字母小写驼峰命名
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
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

#region 数据库创建/更新
if (!File.Exists("wwwroot/data/main.db"))
{
    using var db = DBFactory.InitInstance();
    var sqlStr = await File.ReadAllTextAsync("wwwroot/data/main.sql");
    await db.Ado.ExecuteCommandAsync(sqlStr);
}
if (File.Exists("wwwroot/data/update.sql"))
{
    using var db = DBFactory.InitInstance();
    var sqlStr = await File.ReadAllTextAsync("wwwroot/data/update.sql");
    await db.Ado.ExecuteCommandAsync(sqlStr);
}
PluginHelper.DelForce();
#endregion

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

app.UseCors(option => option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

//将请求与端点匹配，匹配路由
app.UseRouting();

app.MapControllers();

app.Run();