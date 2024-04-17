using FluentScheduler;
using StarBot.Entity;
using StarBot.Extension;
using StarBot.IService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace StarBot.Timer
{
    public class FluentSchedulerFactory : Registry
    {

        ISysConfig _sysConfig;
        ISysIdol _sysIdol;
        public FluentSchedulerFactory()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysIdol = factory.GetService<ISysIdol>()!;
            CreateJob();
        }

        public void CreateJob()
        {
            var config = _sysConfig.GetConfig().Result;
            if (config.EnableModule.WB)
            {
                Schedule(async () => await new Weibo().Save()).WithName("WB").NonReentrant().ToRunEvery(config.WB.TimeSpan).Minutes();
                Schedule(async () => await new Weibo().ChiGua()).WithName("WBChiGua").NonReentrant().ToRunEvery(config.WB.TimeSpan).Minutes();
            }
            if (config.EnableModule.BZ)
            {
                Schedule(async () => await new Bilibili().Monitor()).WithName("BZ").NonReentrant().ToRunEvery(config.WB.TimeSpan).Minutes();
            }
            if (config.EnableModule.XHS)
            {
                Schedule(async () => await new Xhs().Save()).WithName("XHS").NonReentrant().ToRunEvery(config.WB.TimeSpan).Minutes();
            }
            if (config.EnableModule.DY)
            {

            }
            if (config.EnableModule.KD)
            {
            }
            Schedule(async () => await AsyncXox()).ToRunEvery(1).Days().At(0, 0);
        }

        public async Task AsyncXox()
        {
            string url = @"https://fastly.jsdelivr.net/gh/duan602728596/qqtools@main/packages/NIMTest/node/roomId.json";
            try
            {
                HttpClient client = new();
                var str = client.GetStringAsync(url).Result;
                if (str == null) return;
                var res = JObject.Parse(str);
                var arr = JArray.FromObject(res["roomId"]!);
                foreach (var item in arr)
                {
                    SysIdol idol = new()
                    {
                        Id = item["id"]!.ToString(),
                        Name = item["ownerName"]?.ToString() ?? "",
                        RoomId = item["roomId"]?.ToString() ?? "",
                        Account = item["account"]?.ToString() ?? "",
                        ServerId = item["serverId"]?.ToString() ?? "",
                        Team = item["team"]?.ToString() ?? "",
                        TeamId = item["teamId"]?.ToString() ?? "",
                        LiveId = item["liveRoomId"]?.ToString() ?? "",
                        GroupName = item["groupName"]?.ToString() ?? "",
                        PeriodName = item["periodName"]?.ToString() ?? "",
                        PinYin = item["pinyin"]?.ToString() ?? "",
                        ChannelId = item["channelId"]?.ToString() ?? "",
                    };
                    var model = await _sysIdol.GetModelAsync(t => t.Id == idol.Id);
                    if (model == null)
                        await _sysIdol.AddAsync(idol);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
