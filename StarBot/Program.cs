using StarBot.Controllers;
using StarBot.DeskServer;
using StarBot.DeskServer.Models;
using System.Diagnostics;
using System.Net;
using TBC.CommonLib;
using StarBot.IService;
using StarBot.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace StarBot.Desk
{

    internal class Program
    {
        static ISysLog _log;

        [STAThread]
        public static void Main(string[] args)
        {
            var factory = DataService.BuildServiceProvider();
            _log = factory.GetService<ISysLog>()!;

            var conf = File.ReadAllText("appsettings.json").ToJObject();
            var url = conf["Urls"].ToString().Replace("*", "localhost");
            #region 启动API服务
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "dotnet", // 使用 dotnet 命令来运行 .NET Core 控制台应用程序
                    Arguments = "StarBot.API.dll", // 指定子进程的可执行文件路径
                    UseShellExecute = false, // 必须设置为 false，以便在控制台中启动另一个控制台应用程序
                    CreateNoWindow = true,
                };
                // 创建一个 Process 对象，并将 ProcessStartInfo 对象赋给它
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

            #region 启动桌面
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                _log.WriteLog(e.ToString());
            };

            try
            {
                var builder = Application.Initialize();
                Application.AppName = "StarBot";
                Application.Icon = "wwwroot/system/star.png";
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
                _log.WriteLog(ex.Message);
            }
            #endregion
        }
    }
}