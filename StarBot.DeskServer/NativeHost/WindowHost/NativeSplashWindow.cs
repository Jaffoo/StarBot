using StarBot.DeskServer.NativeHost.WindowHost;
using StarBot.DeskServer;
using StarBot.DeskServer.Models;
using System.Drawing;
using System.Reflection;

namespace StarBot.DeskServer.NativeHost.WindowHost
{
    internal class NativeSplashWindow : NativeWindowBase, ISplashWindow
    {
        SplashConfig Option;
        WindowConfig WindowOption;
        internal NativeSplashWindow(SplashConfig option, WindowConfig windowOption)
        {
            Size = option.Size;
            StartupCenter = true;

            Option = option;
            WindowOption = windowOption;
        }
        public override void Create()
        {
            base.Create();
            Handle.DrawImage(Application.FileProvider.GetImage(Option.Splash), 0, 0, Option.Size.Width, Option.Size.Height);
        }
        public override void Show()
        {
            CheckRuntime();
            base.Show();
        }
        public override IntPtr WndProc(IntPtr hwnd, Win32Api.WM message, IntPtr wParam, IntPtr lParam)
        {
            switch (message)
            {
                case Win32Api.WM.GETICON:
                    {
                        return IntPtr.Zero;
                    }
                case Win32Api.WM.CLOSE:
                    {
                        Win32Api.DestroyWindow(hwnd);
                        Handle = IntPtr.Zero;
                        var main = new NativeMainWindow(WindowOption);
                        main.Create();
                        main.Show();
                        Win32Api.PostQuitMessage(0);
                        break;
                    }
                case Win32Api.WM.NCHITTEST:
                    {
                        var result = Win32Api.DefWindowProcW(hwnd, message, wParam, lParam);
                        if (Win32Api.BorderHitTestResults.Contains((Win32Api.HT)result))
                        {
                            return result;
                        }
                        return (IntPtr)Win32Api.HT.CAPTION;
                    }
                case Win32Api.WM.ERASEBKGND:
                    {
                        return IntPtr.Zero;
                    }
                case Win32Api.WM.NCACTIVATE:
                    {
                        return Win32Api.DefWindowProcW(hwnd, message, wParam, new IntPtr(-1));
                    }
            }
            return Win32Api.DefWindowProcW(hwnd, message, wParam, lParam);
        }
        internal override Win32Api.WS GetStyle()
        {
            return Win32Api.WS.CLIPCHILDREN | Win32Api.WS.CLIPSIBLINGS | Win32Api.WS.POPUP | Win32Api.WS.VISIBLE;
        }
        int loadingWidth = 0;
        async Task CheckRuntime()
        {
            int loadingStepWidth = Option.Loading.Width / 100;
            var runtime = await new WebViewHost.WebView2Host.WebView2().CheckWebView(async (progress) =>
            {
                loadingWidth = loadingStepWidth * (int)progress;

                using (var graphics = Graphics.FromHwnd(Handle))
                {
                    graphics.FillRectangle(Brushes.Green, Option.Loading.Left, Option.Loading.Top, loadingWidth, Option.Loading.Height);
                }
            });
            if (!runtime.Succeed)
            {
                Application.MessageBox.ShowError(Handle, $"初始化运行环境失败！{runtime.Message}");
                ExitApplication();
            }
            ShowSplashScreen();
        }

        Timer timer;
        void ShowSplashScreen()
        {
            int loadingInterval = Option.Loading.Delayed / 100;
            int loadingStepWidth = Option.Loading.Width / 100;

            timer = new Timer((state) =>
            {
                if (loadingWidth >= Option.Loading.Width)
                {
                    timer.Dispose();
                    base.Close();
                }
                else
                {
                    loadingWidth += loadingStepWidth;
                    using (var graphics = Graphics.FromHwnd(Handle))
                    {
                        graphics.FillRectangle(Brushes.Green, Option.Loading.Left, Option.Loading.Top, loadingWidth, Option.Loading.Height);
                    }
                }
            }, null, 0, loadingInterval);

        }

        internal override string GetClassName()
        {
            return $"{Assembly.GetEntryAssembly().GetName().Name}.{GetType().Name}";
        }
    }

}

