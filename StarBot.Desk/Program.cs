using StarBot.DeskServer;
using StarBot.DeskServer.Models;
using StarBot.Model;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TBC.CommonLib;

namespace StarBot.Desk
{

    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var conf = File.ReadAllText("appsettings.json").ToJObject();
            var url = conf["Urls"].ToString().Replace("*", "localhost");
            #region 启动API服务
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string directoryPath = Environment.CurrentDirectory; // 要进入的目录路径

                    ProcessStartInfo startInfo = new()
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c cd {directoryPath} && dotnet StarBot.Api.dll",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };
                    Process process = Process.Start(startInfo);
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string directoryPath = Environment.CurrentDirectory; // 要进入的目录路径

                    ProcessStartInfo startInfo = new()
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c 'cd {directoryPath} && dotnet StarBot.Api.dll'",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };
                    Process process = Process.Start(startInfo);
                }
            }
            catch (Exception ex)
            {
                Application.MessageBox.ShowError(nint.Zero, ex.Message);
            }
            #endregion

            #region 启动桌面
            try
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    Application.MessageBox.ShowError(nint.Zero, e.ToString());
                };

                var builder = Application.Initialize();
                Application.AppName = "StarBot";
                Application.Icon =  "wwwroot/system/star.png";
                builder.RegisterResource(typeof(Program));

                var windowConfig = new WindowConfig()
                {
                    Chromeless = false,
                    Size = new System.Drawing.Size(1200, 800),
                    IsDebug = true,
                    WebAppType = WebAppType.Http,
                    Url = url + "/bot/index.html"
                };
                builder.CreateWindow(windowConfig);
                builder.Run();
            }
            catch (Exception ex)
            {
                Application.MessageBox.ShowError(nint.Zero, ex.Message);
            }
            #endregion
        }
    }
}