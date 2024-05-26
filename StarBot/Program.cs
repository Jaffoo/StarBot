using Microsoft.Extensions.DependencyInjection;
using SqlSugar.Extensions;
using StarBot.Controllers;
using StarBot.Extension;
using StarBot.IService;
using System.Diagnostics;
using TBC.CommonLib;
using WinFormium;
using WinFormium.CefGlue;
using WinFormium.Forms;
using WinFormium.JavaScript;

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
                FileName = "StarBot.API.exe",
                WorkingDirectory = Directory.GetCurrentDirectory(),
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
        Url = "http://localhost:5173";

        EnableSplashScreen = false;
        Closing += StarBotUI_Closing;
    }

    private void StarBotUI_Closing(object? sender, ClosingEventArgs e)
    {
        DialogResult res = MessageBox.Show(Owner, "ȷ�Ϲرգ�", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.Yes) e.Cancel = false;
        else e.Cancel = true;
    }

    protected override void OnLoaded(BrowserEventArgs args)
    {
        ExecuteJavaScript($"sessionStorage.setItem('HttpPort','{port}')");
        InjectJavaScript(args.Browser.GetMainFrame());
    }
    protected override void OnBeforeBrowse(BeforeBrowseEventArgs args)
    {
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

    protected void InjectJavaScript(CefFrame frame)
    {
        try
        {
            ShowDevTools();
            var jsInjectHandl = BeginRegisterJavaScriptObject(frame);
            var obj = new JavaScriptObject();
            obj.Add("openDevTool", args =>
            {
                ShowDevTools();
                return true;
            });
            RegisterJavaScriptObject(jsInjectHandl, "devTool", obj);
            EndRegisterJavaScriptObject(jsInjectHandl);

        }
        catch (Exception ex)
        {
            return;
        }
    }
}