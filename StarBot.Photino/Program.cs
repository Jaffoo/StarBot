using Photino.NET;
using StarBot.Controllers;
using StarBot.Helper;
using StarBot.IService;
using System.Diagnostics;
using System.Drawing;
using TBC.CommonLib;
using IWshRuntimeLibrary;
using System.Text;
using System.Reflection;

namespace StarBot.Photino
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            StartAPI();
            Task.Delay(100);
            // Window title declared here for visibility
            string windowTitle = "StarBot";

            // Creating a new PhotinoWindow instance with the fluent API
            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                // Resize to a percentage of the main monitor work area
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(2100, 1350))
                .SetIconFile("logo.ico")
                // Center window in the middle of the screen
                .Center()
                // Users can resize windows by default.
                // Let's make this one fixed instead.
                .SetResizable(true)
                .SetDevToolsEnabled(true);

            string url = "";
            string port = "5266";
            var conf = System.IO.File.ReadAllText("appsettings.json").ToJObject();
            var host = conf["Urls"]?.ToString();
            port = host?.Replace("http://*:", "") ?? port;
            url = host?.Replace("*", "localhost") ?? "http://localhost:" + port;
            // 设置主页地址
            url += "/bot/index.html";
            window.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
                {
                    contentType = "text/javascript";
                    return new MemoryStream(Encoding.UTF8.GetBytes($@"
                        sessionStorage.setItem('HttpPort',{port})
                    "));
                });
            window.Load(url); // Can be used with relative path strings or "new URI()" instance to load a website.

            window.WindowClosing += (sender, e) =>
            {
                //关闭阿里云盘服务
                var alis = Process.GetProcessesByName("alipan-win");
                if (alis.Length > 0)
                    foreach (var item in alis)
                        item.Kill();
                CreateShortIcon(window);
                var res = window.ShowMessage("提示", "确认关闭？", PhotinoDialogButtons.YesNo, PhotinoDialogIcon.Question);
                if (res == PhotinoDialogResult.No)
                    return true;
                return false;
            };
            window.WaitForClose(); // Starts the application event loop           
        }
        private static void StartAPI()
        {
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
                var _log = ConfigHelper.GetService<ISysLog>()!;
                _log.WriteLog(ex.Message);
            }
            #endregion
        }
        private static void CreateShortIcon(PhotinoWindow window)
        {
            //检测并创建桌面快捷方式
            // 获取桌面文件夹路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var link = Path.Combine(desktopPath, "StarBot.lnk");
            if (System.IO.File.Exists(link)) return;
            var res = window.ShowMessage("提示", "检测到桌面没有快捷方式，是否创建？", PhotinoDialogButtons.YesNo, PhotinoDialogIcon.Question);
            if (res == PhotinoDialogResult.No) return;

            // 创建 WshShell 对象
            WshShell shell = new();

            // 创建快捷方式对象
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(link);

            // 设置快捷方式的目标路径
            Assembly assembly = Assembly.GetExecutingAssembly();
            string rootNamespace = assembly.GetName().Name!;
            shortcut.TargetPath = Path.Combine(Directory.GetCurrentDirectory(), rootNamespace + ".exe");

            //设置起始位置，保持工作目录一致
            shortcut.WorkingDirectory = Directory.GetCurrentDirectory();

            // 设置快捷方式的描述
            shortcut.Description = "StarBot.exe";

            // 保存快捷方式
            shortcut.Save();
        }
    }
}
