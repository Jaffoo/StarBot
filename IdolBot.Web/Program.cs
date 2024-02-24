using IdolBot.Helper;
using ElectronSharp.API;
using IdolBot.Extension;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http.Features;
using ElectronSharp.API.Entities;
using System.Diagnostics;

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

builder.Services.AddMvc().AddNewtonsoftJson(options =>
{
    //json序列化设置
    //json序列化设置默认首字母小写驼峰命名
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    //设置时间格式
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
});
//解决razor视图中文被编码
builder.Services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

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
    Height=900,
    Width=1200
});
await browserWindow.WebContents.Session.ClearCacheAsync();
browserWindow.OnReadyToShow += () => browserWindow.Show();
browserWindow.SetTitle("测试-" + port);
#endregion

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Urls.Add("http://localhost:" + port);

app.Run();