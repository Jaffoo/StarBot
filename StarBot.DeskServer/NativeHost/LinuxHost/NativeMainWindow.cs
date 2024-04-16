using System.Diagnostics;
using System.Runtime.InteropServices;
using StarBot.DeskServer.WebViewHost.GtkWebkitHost;
using StarBot.DeskServer;
using StarBot.DeskServer.Models;

namespace StarBot.DeskServer.NativeHost.LinuxHost
{
    internal class NativeMainWindow : NativeWindowBase, IWindow
    {
        public Type BlazorComponent { get; set; }
        public string BlazorSelector { get; set; }
        private IWebView webView;

        string Url;

        public NativeMainWindow(WindowConfig option)
        {
            MaximumSize = option.MaximumSize;
            MinimumSize = option.MinimumSize;
            Size = option.Size;
            Chromeless = option.Chromeless;
            CanMinMax = option.CanMinMax;
            CanReSize = option.CanReSize;
            StartupCenter = option.StartupCenter;
            WindowState = option.WindowState;
            Url = option.Url;
            isDebug = option.IsDebug;
            webAppType = option.WebAppType;

            BlazorComponent = option.BlazorComponent;
            BlazorSelector = option.BlazorSelector;

            OnSizeChange += (s, e) =>
            {
                //  webView.SizeChange(Handle, e.Width, e.Height);
            };
        }

        private bool isDebug;
        private WebAppType webAppType;
        public override void Show()
        {
            webView = new GtkWebkit();
            webView.SetDebug(isDebug);
            webView.SetWebAppType(webAppType);
            webView.Initialization(this, Size.Width, Size.Height, Url, BlazorComponent, BlazorSelector);
  
            RegisterHandel(webView.Handle, "button-press-event", (widget, ev, data) => base.OnButtonPressEvent(Handle, ev, data));
            RegisterHandel(webView.Handle, "button-release-event", (widget, ev, data) => base.OnButtonReleaseEvent(Handle, ev, data));

            base.Show();
        }
        public override void Close()
        {
            ExitApplication();
        }
        public void OpenSystemBroswer(string url)
        {
            var process = new Process();
            process.StartInfo.FileName = "xdg-open";
            process.StartInfo.Arguments = url;
            process.Start();
        }
    }
}