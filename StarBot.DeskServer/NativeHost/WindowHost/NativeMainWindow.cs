﻿using StarBot.DeskServer.NativeHost.WindowHost;
using StarBot.DeskServer.WebViewHost.WebView2Host;
using StarBot.DeskServer.Models;
using System.Reflection;
using System.Runtime.InteropServices;
using StarBot.DeskServer.NativeHost.WindowHost;

namespace StarBot.DeskServer.NativeHost.WindowHost
{
    internal class NativeMainWindow : NativeWindowBase, IWindow
    {
        public Type BlazorComponent { get; set; }
        public string BlazorSelector { get; set; }
        private readonly IWebView webView;

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

            BlazorComponent = option.BlazorComponent;
            BlazorSelector = option.BlazorSelector;
            webView = new WebView2();
            webView.SetDebug(option.IsDebug);
            webView.SetWebAppType(option.WebAppType);


            OnSizeChange += (s, e) =>
            {
                webView.SizeChange(Handle, e.Width, e.Height);
            };
        }

        public override IntPtr WndProc(IntPtr hwnd, Win32Api.WM message, IntPtr wParam, IntPtr lParam)
        {
            if (Chromeless)
            {
                switch (message)
                {
                    case Win32Api.WM.NCPAINT:
                        return IntPtr.Zero;
                    case Win32Api.WM.NCACTIVATE:
                        return Win32Api.DefWindowProcW(hwnd, message, wParam, new IntPtr(-1));
                    case Win32Api.WM.NCCALCSIZE:
                        {
                            var result = Win32Api.DefWindowProcW(hwnd, message, wParam, lParam);
                            var csp = (Win32Api.NcCalcSizeParams)Marshal.PtrToStructure(lParam, typeof(Win32Api.NcCalcSizeParams));
                            csp.Region.Input.TargetWindowRect.Top -= GetTopFrameHeight();
                            Marshal.StructureToPtr(csp, lParam, false);
                            return result;
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
                }
            }
            return base.WndProc(hwnd, message, wParam, lParam);
        }



        public override void Create()
        {
            base.Create();

            var size = GetClientSize();
            webView.Initialization(this, size.Width(), size.Height() + GetTopFrameHeight(), Url, BlazorComponent, BlazorSelector);
        }
        public override void Show()
        {
            base.Show();
        }
        internal override string GetClassName()
        {
            return $"{Assembly.GetEntryAssembly().GetName().Name}.{GetType().Name}";
        }

        public void OpenSystemBroswer(string url)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { FileName = url, UseShellExecute = true });
        }
    }
}

