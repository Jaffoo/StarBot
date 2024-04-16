using StarBot.Helper;
using StarBot.Extension;
using Microsoft.AspNetCore.Http.Features;
using StarBot.Repository;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using FluentScheduler;
using StarBot.Timer;

var builder = WebApplication.CreateBuilder(args);

//���û�������
ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
//ע������
var projectName = ConfigHelper.GetConfiguration("NameSpace");
builder.Services.AddDataService(projectName + ".Service");

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
PluginHelper.DelForce();
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

app.Run();