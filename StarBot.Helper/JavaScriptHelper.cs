using ElectronNET.API;

namespace StarBot.Helper
{
    public static class JavaScriptHelper
    {
        public static async void InjectJavaScript(BrowserWindow window, string jsCode)
        {
            try
            {
                await window.WebContents.ExecuteJavaScriptAsync(jsCode);
            }
            catch
            {
                throw;
            }
        }
    }
}
