using StarBot.DeskServer;
using StarBot.DeskServer.Models;
using StarBot.Model;
using System.Diagnostics;
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
                string serviceName = "StarBot.API";
                if (Process.GetProcessesByName(serviceName).Length == 0)
                {
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = serviceName + ".exe",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };
                    Process process = Process.Start(startInfo);
                }
                var res = Tools.GetAsync<ApiResult>(url).Result;
                if (!res.Success)
                {
                    Application.MessageBox.ShowError(nint.Zero, "服务启动失败！请重新启动尝试。");
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
                Application.Icon = "wwwroot/bot/star.jpg";
                builder.RegisterResource(typeof(Program));

                var windowConfig = new WindowConfig()
                {
                    Chromeless = false,
                    Size = new System.Drawing.Size(1200, 800),
                    IsDebug = false,
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