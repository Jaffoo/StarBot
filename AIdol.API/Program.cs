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
using System.Net.NetworkInformation;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

//���û�������
ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
//ע������
builder.Services.AddDataService(ConfigHelper.GetConfiguration("NameSpace") + ".Service");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;//�����˴���ǰ��ȫ��д
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);//�����˷����������ı�����
    }).AddNewtonsoftJson(options =>
    {
        //json���л�����
        //json���л�����Ĭ������ĸСд�շ�����
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        //����ʱ���ʽ
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    });

//�ļ��ϴ���С
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});
//����
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

var app = builder.Build();

#region ���ݿⴴ��/����
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

//������ʱ����
JobManager.Initialize(new FluentSchedulerFactory());

//��ȡ����ע��ķ���
ConfigHelper.ServiceProvider = app.Services;

//�Զ����쳣�����м��
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    //���ò�����content-type
    ServeUnknownFileTypes = true
});

app.UseCors(option => option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

//��������˵�ƥ�䣬ƥ��·��
app.UseRouting();

app.MapControllers();

var portsStr = ConfigHelper.GetConfiguration("Ports")!;
var ports = portsStr.Split("|").Select(int.Parse).ToList();
IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
List<int> activePorts = tcpEndPoints.Select(ep => ep.Port).ToList();
List<int> availablePorts = ports.Where(port => !activePorts.Contains(port)).ToList();
var port = availablePorts.FirstOrDefault();
app.Urls.Add($"http://localhost:{port}");
//var startUrl = $"http://localhost:{port}/aidol/index.html";
var startUrl = $"http://localhost:5173";
await app.StartAsync();
var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
{
    AutoHideMenuBar = true,
    Height = 800,
    Width = 1000,
    WebPreferences = new WebPreferences
    {
        DevTools = true,//Ctrl + Shift + I
        NodeIntegration = true,
    }
}, startUrl);
await browserWindow.WebContents.Session.ClearCacheAsync();
browserWindow.OnReadyToShow += () =>
{
    browserWindow.Show();
    browserWindow.SetTitle("ElectronAPI-" + port);
    JavaScriptHelper.InjectJavaScript(browserWindow, $"sessionStorage.setItem('HttpPort',{port})");
};
app.WaitForShutdown();