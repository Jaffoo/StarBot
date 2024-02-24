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

//���û�������
ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
//ע������
builder.Services.AddDataService(ConfigHelper.GetConfiguration("NameSpace") + ".Service");

builder.Services.AddMvc().AddNewtonsoftJson(options =>
{
    //json���л�����
    //json���л�����Ĭ������ĸСд�շ�����
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    //����ʱ���ʽ
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
});
//���razor��ͼ���ı�����
builder.Services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

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


#region electron
var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
{
    AutoHideMenuBar = true,
    Height=900,
    Width=1200
});
await browserWindow.WebContents.Session.ClearCacheAsync();
browserWindow.OnReadyToShow += () => browserWindow.Show();
browserWindow.SetTitle("����-" + port);
#endregion

var app = builder.Build();

//��ȡ����ע��ķ���
ConfigHelper.ServiceProvider = app.Services;

//�Զ����쳣�����м��
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    //���ò�����content-type
    ServeUnknownFileTypes = true
});

//��������˵�ƥ�䣬ƥ��·��
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Urls.Add("http://localhost:" + port);

app.Run();