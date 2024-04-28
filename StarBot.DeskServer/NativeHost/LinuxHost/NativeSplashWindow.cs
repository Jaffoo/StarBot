namespace StarBot.DeskServer.NativeHost.LinuxHost
{
    internal class NativeSplashWindow : NativeWindowBase, ISplashWindow
    {
        SplashConfig Option;
        WindowConfig WindowOption;

        internal NativeSplashWindow(SplashConfig option, WindowConfig windowOption)
        {
            LinuxExtensions.InitGtk();
            Chromeless = true;
            Size = option.Size;
            StartupCenter = true;

            Option = option;
            WindowOption = windowOption;
        }

        public override void Create()
        {
            base.Create();
            var splashPtr = Application.FileProvider.GetFileInfo(Option.Splash).CreateReadStream().GetImageIntPtr();
            GtkApi.gtk_container_add(Handle, splashPtr);
            GtkApi.gtk_widget_show(splashPtr);
        }

        public override void Show()
        {
            base.Show();
            Task.Run(() => { showSplashScreen(); });
            RunMessageLoop();
            var main = new NativeMainWindow(WindowOption);
            main.Create();
            main.Show();
            main.RunMessageLoop();
        }

        Timer timer;

        int loadingWidth = 0;

        void showSplashScreen()
        {

            var color = Option.Loading.Color.HtmlColorToRgb();
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
                    IntPtr window = GtkApi.gtk_widget_get_window(Handle);
                    IntPtr cr = GtkApi.gdk_cairo_create(window);
                    GtkApi.cairo_rectangle(cr, Option.Loading.Left, Option.Loading.Top, loadingWidth, Option.Loading.Height);
                    GtkApi.cairo_set_source_rgb(cr, color.R, color.G, color.B);
                    GtkApi.cairo_fill(cr);
                }
            }, null, 0, loadingInterval);
        }
    }
}