using AIdol.Helper;
using ElectronNET.API;
using AIdol.Extension;
using Microsoft.AspNetCore.Http.Features;
using ElectronNET.API.Entities;
using AIdol.Repository;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using FluentScheduler;
using AIdol.Timer;
using TBC.CommonLib;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

//设置基础配置
ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
//注入数据
builder.Services.AddDataService(ConfigHelper.GetConfiguration("NameSpace") + ".Service");

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

//将请求与端点匹配，匹配路由
app.UseRouting();

app.MapControllers();

var port = 6051;
app.Urls.Add($"http://localhost:{port}");
var startUrl = $"http://localhost:{port}/aidol/index.html";
//var startUrl = $"http://localhost:5173";
await app.StartAsync();
var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
{
    AutoHideMenuBar = true,
    Height = 900,
    Width = 1200
}, startUrl);
await browserWindow.WebContents.Session.ClearCacheAsync();
browserWindow.OnReadyToShow += () => browserWindow.Show();
browserWindow.SetTitle("ElectronAPI-" + port);

app.WaitForShutdown();