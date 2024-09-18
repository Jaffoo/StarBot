using Photino.NET;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.Text;
using System.Reflection;
using System.Net.Sockets;

namespace StarBot.Photino;

class Program
{
    private static PhotinoWindow? Window;
    [STAThread]
    static void Main()
    {
        Window = new PhotinoWindow()
        {
            Title = "StarBot",
            IconFile = "logo.ico",
            Centered = true,
            UseOsDefaultSize = false,
            Width = 2100,
            Height = 1350,
            MediaAutoplayEnabled = true,
            ContextMenuEnabled = false,
        };
        Window.LoadRawString("</br>");

        StartAPI();

        Window.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
            {
                contentType = "text/javascript";
                return new MemoryStream(Encoding.UTF8.GetBytes($@"
                        sessionStorage.setItem('HttpPort',5266)
                    "));
            });

        Window.WindowClosing += (sender, e) =>
        {
            //关闭阿里云盘服务
            var alis = Process.GetProcessesByName("alipan-win");
            if (alis.Length > 0)
                foreach (var item in alis)
                    item.Kill();
            CreateShortIcon();
            var res = Window.ShowMessage("提示", "确认关闭？", PhotinoDialogButtons.YesNo, PhotinoDialogIcon.Question);
            if (res == PhotinoDialogResult.No)
                return true;
            return false;
        };

        Window.WaitForClose(); // Starts the application event loop           
    }
    private static void StartAPI()
    {
        // 启动 Web API服务
        var host = "localhost";
        var port = 5266;
        if (!SeverIsUp(host, port))
        {
            var apiApp = Api.Program.InitAPI();
            Task.Run(() => apiApp.Run($"http://{host}:{port}"));
            while (true)
            {
                if (SeverIsUp(host, port))
                {
                    Window!.Load($"http://{host}:{port}/bot/index.html?v=" + DateTime.Now.ToString("yyyyMMdd"));
                    break;
                }
                Thread.Sleep(1);
            }
        }
        else Window!.Load($"http://{host}:{port}/bot/index.html?v=" + DateTime.Now.ToString("yyyyMMdd"));
    }
    private static void CreateShortIcon()
    {
        //检测并创建桌面快捷方式
        // 获取桌面文件夹路径
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var link = Path.Combine(desktopPath, "StarBot.lnk");
        if (System.IO.File.Exists(link)) return;
        var res = Window!.ShowMessage("提示", "检测到桌面没有快捷方式，是否创建？", PhotinoDialogButtons.YesNo, PhotinoDialogIcon.Question);
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

    private static bool SeverIsUp(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var result = client.BeginConnect(host, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)); // 设置超时时间

            if (!success)
            {
                return false; // 连接超时
            }

            client.EndConnect(result);
            return true; // 成功连接
        }
        catch (SocketException)
        {
            return false; // 连接失败
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生异常: {ex.Message}");
            return false;
        }
    }

}
