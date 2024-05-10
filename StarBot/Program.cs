using Microsoft.Extensions.DependencyInjection;
using StarBot.Controllers;
using StarBot.Extension;
using StarBot.IService;
using System.Diagnostics;
using TBC.CommonLib;
using WinFormium;
using WinFormium.Forms;

namespace StarBot;
internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var builder = WinFormiumApp.CreateBuilder();

        builder.UseWinFormiumApp<App>();
#if DEBUG
        builder.UseDevToolsMenu();
#endif
        var app = builder.Build();

        app.Run();
    }
}

internal class App : WinFormiumStartup
{
    protected override MainWindowCreationAction? UseMainWindow(MainWindowOptions opts)
    {
        // ����Ӧ�ó����������
        return opts.UseMainFormium<StarBotUI>();
    }

    protected override void WinFormiumMain(string[] args)
    {
        var factory = DataService.BuildServiceProvider();
        var _log = factory.GetService<ISysLog>()!;
        #region ����API����
        try
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet", // ʹ�� dotnet ���������� .NET Core ����̨Ӧ�ó���
                Arguments = "StarBot.API.dll", // ָ���ӽ��̵Ŀ�ִ���ļ�·��
                UseShellExecute = false, // ��������Ϊ false���Ա��ڿ���̨��������һ������̨Ӧ�ó���
                CreateNoWindow = true,
            };
            // ����һ�� Process ���󣬲��� ProcessStartInfo ���󸳸���
            Process process = new()
            {
                StartInfo = startInfo
            };
            process.Start();

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
                if (HomeController.AliProcess != null)
                {
                    HomeController.AliProcess.Kill();
                    HomeController.AliProcess = null;
                }
            };
        }
        catch (Exception ex)
        {
            _log.WriteLog(ex.Message);
        }
        #endregion

        // Main�����еĴ���Ӧ��������ú���ֻ�������������С��������Է�ֹ�ӽ�������һЩ����ȷ�ĳ�ʼ�����롣
        ApplicationConfiguration.Initialize();
    }
}

internal class StarBotUI : Formium
{
    string url = "";
    string port = "5266";
    public StarBotUI()
    {
        var conf = File.ReadAllText("appsettings.json").ToJObject();
        var host = conf["Urls"]?.ToString();
        url = host?.Replace("*", "localhost") ?? "http://localhost:" + port;
        port = host?.Replace("http://*:", "") ?? port;
        // ������ҳ��ַ
        Url = url + "/bot/index.html";

        EnableSplashScreen = false;
    }

    protected override void OnBeforeBrowse(BeforeBrowseEventArgs args)
    {
        ExecuteJavaScript($"sessionStorage.setItem('HttpPort','{port}')");
#if DEBUG
        ShowDevTools();
#endif
    }

    protected override FormStyle ConfigureWindowStyle(WindowStyleBuilder builder)
    {
        // �˴����ô������ʽ�����ԣ��򲻼̳д˷�����ʹ��Ĭ����ʽ��
        var style = builder.UseSystemForm();
        style.Size = new Size(1200, 800);
        style.TitleBar = true;
        style.Icon = new Icon("logo.ico");
        style.DefaultAppTitle = "StarBot";
        style.ColorMode = FormiumColorMode.SystemPreference;
        style.StartCentered = StartCenteredMode.CenterScreen;
        return style;
    }
}