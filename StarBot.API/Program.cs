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
    /// ��ʼ��
    /// </summary>

    [STAThread]
    public static void Main() => InitAPI().Run();

    /// <summary>
    /// ��ʼ��api
    /// </summary>
    public static WebApplication InitAPI()
    {
        var builder = WebApplication.CreateBuilder();

        //���û�������
        ConfigHelper.SetConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath);
        //ע������
        var projectName = ConfigHelper.GetConfiguration("NameSpace");
        builder.Services.AddDataService(projectName + ".Service");

        //ͬ�����ݿ�ṹ
        DBFactory.InitTable();

        builder.Services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;//�����˴���ǰ��ȫ��д
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);//�����˷����������ı�����
            }).AddNewtonsoftJson(options =>
            {
                //json���л�����
                //json���л�����Ĭ������ĸСд�շ�����
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
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
        //�����������ã��˷����������нӿڿ���
        app.UseCors(builder => builder
            //�����κ���Դ
            .AllowAnyOrigin()
             //�������󷽷�
             .AllowAnyMethod()
             //��������ͷ
             .AllowAnyHeader());

        app.UseCors(option => option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        //��������˵�ƥ�䣬ƥ��·��
        app.UseRouting();

        app.MapControllers();

        return app;
    }
}