using AIdol.Helper;
using ElectronSharp.API;
using AIdol.Extension;
using Microsoft.AspNetCore.Http.Features;
using ElectronSharp.API.Entities;
using System.Diagnostics;
using AIdol.Repository;

var previousProcesses = Process.GetProcessesByName("electron");
foreach (var p in previousProcesses)
    p.Kill();

var port = Electron.Experimental.FreeTcpPort();
await Electron.Experimental.StartElectronForDevelopment(port);

var builder = WebApplication.CreateBuilder(args);

//设置基础配置
ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
//注入数据
builder.Services.AddDataService(ConfigHelper.GetConfiguration("NameSpace") + ".Service");

builder.Services.AddControllers().AddNewtonsoftJson(options =>
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

#region electron
var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
{
    AutoHideMenuBar = true,
    Height = 900,
    Width = 1200
}, $"http://localhost:{port}/aidol/index.html");
await browserWindow.WebContents.Session.ClearCacheAsync();
browserWindow.OnReadyToShow += () => browserWindow.Show();
browserWindow.SetTitle("测试-" + port);
#endregion

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

app.Urls.Add($"http://localhost:{port}");

app.Run();