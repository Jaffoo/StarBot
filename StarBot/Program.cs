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
        // 设置应用程序的主窗体
        return opts.UseMainFormium<StarBotUI>();
    }

    protected override void WinFormiumMain(string[] args)
    {
        var factory = DataService.BuildServiceProvider();
        var _log = factory.GetService<ISysLog>()!;
        #region 启动API服务
        try
        {
            Process process = new();
            //先检测服务存在不存在
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
                // 创建一个 Process 对象，并将 ProcessStartInfo 对象赋给它
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

        // Main函数中的代码应该在这里，该函数只在主进程中运行。这样可以防止子进程运行一些不正确的初始化代码。
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
        // 设置主页地址
        Url = url + "/bot/index.html";

        EnableSplashScreen = false;
        Closing += StarBotUI_Closing;
    }

    private void StarBotUI_Closing(object? sender, ClosingEventArgs e)
    {
        DialogResult res = MessageBox.Show(Owner, "确认关闭！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        // 此处配置窗体的样式和属性，或不继承此方法以使用默认样式。
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
        //检测并创建桌面快捷方式
        // 获取桌面文件夹路径
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var link = Path.Combine(desktopPath, "StarBot.lnk");
        if (System.IO.File.Exists(link)) return;
        DialogResult res = MessageBox.Show(Owner, "检测到桌面没有快捷方式，是否创建？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.No) return;

        // 创建 WshShell 对象
        WshShell shell = new();

        // 创建快捷方式对象
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(link);

        // 设置快捷方式的目标路径
        shortcut.TargetPath = Path.Combine(Directory.GetCurrentDirectory(), "StarBot.exe");

        //设置起始位置，保持工作目录一致
        shortcut.WorkingDirectory = Directory.GetCurrentDirectory();

        // 设置快捷方式的描述
        shortcut.Description = "StarBot.exe";

        // 保存快捷方式
        shortcut.Save();
    }
}