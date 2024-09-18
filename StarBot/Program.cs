using Microsoft.Extensions.DependencyInjection;
using IWshRuntimeLibrary;
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
            Process process = new();
            //�ȼ�������ڲ�����
            var list = Process.GetProcessesByName("StarBot.API");
            if (list.Length == 1)
            {
                process = list[0];
            }
            if (list.Length > 1)
            {
                foreach (var item in list)
                {
                    item.Kill();
                    list = [];
                }
            }
            if (list?.Length <= 0)
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "StarBot.API.exe",
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    CreateNoWindow = true,
                };
                // ����һ�� Process ���󣬲��� ProcessStartInfo ���󸳸���
                process = new()
                {
                    StartInfo = startInfo
                };
                process.Start();
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                if (!process.HasExited)
                {
                    process.Kill();
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
        var conf = System.IO.File.ReadAllText("appsettings.json").ToJObject();
        var host = conf["Urls"]?.ToString();
        port = host?.Replace("http://*:", "") ?? port;
        url = host?.Replace("*", "localhost") ?? "http://localhost:" + port;
        // ������ҳ��ַ
        Url = url + "/bot/index.html";

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
        Task.Run(CreateShortIcon);
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

    private void CreateShortIcon()
    {
        //��Ⲣ���������ݷ�ʽ
        // ��ȡ�����ļ���·��
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var link = Path.Combine(desktopPath, "StarBot.lnk");
        if (System.IO.File.Exists(link)) return;
        DialogResult res = MessageBox.Show(Owner, "��⵽����û�п�ݷ�ʽ���Ƿ񴴽���", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.No) return;

        // ���� WshShell ����
        WshShell shell = new();

        // ������ݷ�ʽ����
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(link);

        // ���ÿ�ݷ�ʽ��Ŀ��·��
        shortcut.TargetPath = Path.Combine(Directory.GetCurrentDirectory(), "StarBot.exe");

        //������ʼλ�ã����ֹ���Ŀ¼һ��
        shortcut.WorkingDirectory = Directory.GetCurrentDirectory();

        // ���ÿ�ݷ�ʽ������
        shortcut.Description = "StarBot.exe";

        // �����ݷ�ʽ
        shortcut.Save();
    }
}