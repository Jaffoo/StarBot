using AIdol.IService;
using Newtonsoft.Json.Linq;
using TBC.CommonLib;
using ShamrockCore.Receiver.MsgChain;
using AIdol.Extension;
using Microsoft.Extensions.DependencyInjection;
using Config = AIdol.Model.Config;

namespace AIdol.Extension
{
    public class Bilibili
    {
        ISysLog _sysLog;
        ISysConfig _sysConfig;
        Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }

        public Bilibili()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
        }
        private static Dictionary<string, string> GetHeader(string uid)
        {
            return new Dictionary<string, string>()
            {
                {":authority","api.vc.bilibili.com" },
                {":method","GET" },
                {":path","/dynamic_svr/v1/dynamic_svr/space_history?host_uid="+uid },
                {":scheme","https" },
                {"Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
                {"Accept-Language","zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6" },
                {"Accept-Encoding","gzip, deflate, br" },
                {"Cache-Control","max-age=0" },
                {"Cookie","l=v; buvid3=8B933317-129B-93B2-3937-C1478234151627154infoc; b_nut=1698491127; _uuid=D17F5555-68ED-10956-52B8-747310CE43C1127223infoc; buvid_fp=1f4fd64873d2c48a4ed5a6a2f275ee3a; buvid4=55FF8673-F66A-18B0-EB55-8A7E0D1ED3B428883-023102819-7TXVQ7Yxm0luGv4FVXOmMo9ubwmd5yF%2FbbJfqADmEwi%2BUnQ1tKoCug%3D%3D; enable_web_push=DISABLE; header_theme_version=CLOSE; home_feed_column=5; PVID=1; innersign=0; b_lsid=E16DE115_18BB2B6359C; bsource=search_baidu; browser_resolution=1455-759; bili_ticket=eyJhbGciOiJIUzI1NiIsImtpZCI6InMwMyIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2OTk3Njk1NzMsImlhdCI6MTY5OTUxMDMxMywicGx0IjotMX0.l8xhCnSF3xWd45A2C8OahnhQJSu9ErbxYjwu4owNqpM; bili_ticket_expires=1699769513" },
                {"Sec-Ch-Ua","\"Google Chrome\";v=\"117\", \"Not;A=Brand\";v=\"8\", \"Chromium\";v=\"117\"" },
                {"Sec-Ch-Ua-Mobile","?0" },
                {"ec-Ch-Ua-Platform","\"Windows\"" },
                {"Sec-Fetch-Dest","document" },
                {"Sec-Fetch-Mode","navigate" },
                {"Sec-Fetch-Site","none" },
                {"Sec-Fetch-User","?1" },
                {"Upgrade-Insecure-Requests","1" },
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36" },
            };
        }
        public async Task Monitor()
        {
            try
            {
                var config = (await _sysConfig.GetConfig());
                var index = -1;
                var users = config.BZ.User.ToStrList();
                foreach (var item in users)
                {
                    index++;
                    var url = "https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history?host_uid=" + item;
                    HttpClient httpClient = new();
                    var headers = GetHeader(item);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    var res = await httpClient.GetAsync(url);
                    var content = await res.Content.ReadAsStringAsync();
                    var data = JObject.Parse(content);
                    var code = data["code"]!.ToString();
                    if (code != "0") continue;
                    var list = JArray.FromObject(data["data"]!["cards"]!);
                    foreach (JObject blog in list.Cast<JObject>())
                    {
                        var timestamp = blog["desc"]!["timestamp"]!.ToString().ToLong();
                        DateTime createDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        createDate = createDate.AddSeconds(timestamp).ToLocalTime();

                        if (createDate >= DateTime.Now.AddMinutes(-config.BZ.TimeSpan))
                        {
                            //可以认定为新发的动态
                            //获取微博类型0-视频，1-图文
                            var type = -1;
                            if (blog["desc"]!["type"]!.ToString() == "2") type = 1;
                            else if (blog["desc"]!["type"]!.ToString() == "8") type = 0;
                            //需要发送通知则发送通知
                            if (index == 0)
                            {
                                var mcb = new MessageChainBuilder();
                                var msgModel = new MsgModel
                                {
                                    MsgStr = $"{blog["desc"]!["user_profile"]!["info"]!["uname"]}B站更新："
                                };
                                //预留是否要at所有人
                                //if (false)
                                //{
                                //    mcb.AtAll();
                                //}
                                mcb.Text(msgModel.MsgStr);
                                var card = JObject.Parse(blog["card"]!.ToString());
                                if (type == 1)
                                {
                                    //获取第一张图片发送
                                    var imgList = JArray.FromObject(card!["item"]!["pictures"]!);
                                    mcb.Text(card!["item"]!["description"]!.ToString()).ImageByUrl(imgList[0]["img_src"]!.ToString());
                                }
                                else if (type == 0)
                                {
                                    mcb.Text(card!["title"]!.ToString())
                                    .Text("\nBV号:" + blog["desc"]!["bvid"] + "\n直达:https://www.bilibili.com/video/" + blog["desc"]!["bvid"])
                                    .ImageByUrl(card["pic"]?.ToString() ?? "");
                                }
                                else
                                {
                                    if (card.ContainsKey("title"))
                                    {
                                        mcb.Text(card!["title"]!.ToString());
                                    }
                                    else
                                    {
                                        mcb.Text(card!["item"]!["description"]!.ToString());
                                    }
                                }

                                if (Config.BZ.ForwardGroup)
                                {
                                    var goups = Config.BZ.Group ?? Config.QQ.Group;
                                    if (goups == null) continue;
                                    await ReciverMsg.Instance.SendGroupMsg(goups.ToStrList(), mcb.Build());
                                }
                                if (Config.BZ.ForwardQQ)
                                {
                                    if (string.IsNullOrWhiteSpace(Config.BZ.QQ)) continue;
                                    await ReciverMsg.Instance.SendFriendMsg(Config.BZ.QQ.ToStrList(), mcb.Build());
                                }
                            }
                            //保存图片
                            if (type == 1)
                            {
                                var card = JObject.Parse(blog["card"]!.ToString());
                                var picList = JArray.FromObject(card!["item"]!["pictures"]!);
                                if (picList == null) continue;
                                foreach (var pic in picList)
                                {
                                    var imgUrl = pic["img_src"]!.ToString();
                                    await new Weibo().FatchFace(imgUrl);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return;
            }
        }
    }
}
