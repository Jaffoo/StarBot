using AIdol.Extension;
using AIdol.IService;
using Microsoft.Extensions.DependencyInjection;

namespace AIdol.Extension
{
    public class AliYun
    {
        ISysLog _sysLog ;
        ISysConfig _sysConfig ;

        public AliYun()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
        }

        public async Task<string> Upload(string path)
        {
            try
            {
                var config = await _sysConfig.GetConfig();
                HttpClient httpClient = new();
                return await httpClient.GetStringAsync("http://127.0.0.1:5555/ali/upload?path=" + path + "&album=" + config.BD.AlbumName);
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return "{'msg':'" + e.Message + "'}";
            }
        }

        public async Task<string> GetList()
        {
            try
            {
                var config = await _sysConfig.GetConfig();
                HttpClient httpClient = new();
                return await httpClient.GetStringAsync("http://127.0.0.1:5555/ali/getalbumphotos?album=" + config.BD.AlbumName);

            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return "{'msg':'" + e.Message + "'}";
            }
        }
    }
}
