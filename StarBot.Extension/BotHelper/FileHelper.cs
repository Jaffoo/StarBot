using StarBot.Extension;
using StarBot.IService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace StarBot.Extension
{
    public class FileHelper
    {
        ISysLog _sysLog;
        ISysConfig _sysConfig;

        public FileHelper()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
        }
        public async Task<bool> Save(string url)
        {
            try
            {
                var config = await _sysConfig.GetConfig();
                var path = url;
                if (path.Contains("https://") || path.Contains("http://"))
                {
                    HttpClient client = new();
                    byte[] bytes = client.GetByteArrayAsync(path).Result;
                    var root = Directory.GetCurrentDirectory() + @"/wwwroot/images/";
                    if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                    path = root + "66-" + DateTime.Now.ToString("yyMMddHHmmssfff") + ".jpeg";
                    File.WriteAllBytes(path, bytes);
                }
                if (config.EnableModule.BD && config.BD.SaveAliyunDisk)
                {
                    ThreadStart ts = new(async () =>
                    {
                        //存入阿里云盘
                        AliYun aliCloud = new();
                        var res = await aliCloud.Upload(path);
                        var data = JObject.Parse(res);
                        var msg = data["msg"]?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(msg)) msg = "保存状态未知，请前往云盘查看!";
                        await ReciverMsg.Instance.SendAdminMsg(msg);
                    });
                    Thread t = new(ts);
                    t.Start();
                }
                return true;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return false;
            }
        }
        public async Task<string> SaveLocal(string url)
        {
            try
            {
                HttpClient client = new();
                byte[] bytes = client.GetByteArrayAsync(url).Result;
                var root = Directory.GetCurrentDirectory() + @"/wwwroot/images/";
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                var path = root + "66-" + DateTime.Now.ToString("yyMMddHHmmssfff") + ".jpeg";
                File.WriteAllBytes(path, bytes);
                return path;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return "";
            }
        }
    }
}
