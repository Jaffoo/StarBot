using StarBot.Entity;
using StarBot.IService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TBC.CommonLib;
using Config = StarBot.Model.Config;
using UnifyBot.Message.Chain;

namespace StarBot.Extension
{
    public class Pocket
    {
        private static Pocket? instance;
        private static readonly object lockObject = new object();
        ISysConfig _sysConfig;
        ISysLog _sysLog;
        IPoketMessage _pocketMsg;

        private Pocket()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
            _pocketMsg = factory.GetService<IPoketMessage>()!;
        }

        public static Pocket Instance
        {
            get
            {
                lock (lockObject)
                {
                    instance ??= new Pocket();
                    return instance;
                }
            }
        }
        private Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }
        public async Task PocketMessageReceiver(string str)
        {
            try
            {
                if (!Config.EnableModule.KD) return;
                var result = JObject.Parse(str);
                var time = result["time"]!.ToString();
                var channelName = result["channelName"]!.ToString();
                var name = result["ext"]!["user"]!["nickName"]!.ToString();
                int roleId = result["ext"]!["user"]!["roleId"]!.ToString().ToInt();
                string msgType = result["type"]!.ToString();
                string msbBody = "";

                //保存信息
                var saveMsg = new PoketMessage()
                {
                    Time = time.ToDateTime(),
                    ChannelRole = roleId,
                    ChannelName = channelName,
                    FullInfo = str,
                    Type = msgType,
                };

                MessageChainBuild mcb = new();
                mcb.Text($"【{GetIdolName(result["serverId"]!.ToString())}|{channelName}】\n【{time}】\n{name}:");
                //图片
                if (msgType == "image")
                {
                    msbBody = result["attach"]!["url"]!.ToString();
                    if (Config.KD.SaveImg)
                    {
                        await Task.Run(async () =>
                        {
                            await new Baidu().FatchFace(msbBody, "口袋");
                        });
                    }
                    if (!Config.KD.MsgType.Contains(msgType)) return;
                    mcb.ImageByUrl(msbBody);
                    saveMsg.Msg = msbBody;
                }
                //文字
                else if (Config.KD.MsgType.Contains(msgType) && msgType == "text")
                {
                    //"230226137"
                    msbBody = result["body"]!.ToString();
                    mcb.Text(msbBody);
                    saveMsg.Msg = msbBody;
                }
                //视频
                else if (Config.KD.MsgType.Contains(msgType) && msgType == "video")
                {
                    msbBody = result["attach"]!["url"]!.ToString();
                    mcb.Text(msbBody);
                    saveMsg.Msg = msbBody;
                }
                //语言
                else if (Config.KD.MsgType.Contains(msgType) && msgType == "audio")
                {
                    msbBody = result["attach"]!["url"]!.ToString();
                    mcb.RecordByUrl(msbBody);
                    saveMsg.Msg = msbBody;
                }
                else if (msgType == "custom")
                {
                    var attach = result["attach"]!;
                    var messageType = attach["messageType"]!.ToString();
                    //回复
                    if (Config.KD.MsgType.Contains(messageType) && messageType == "REPLY")
                    {
                        msbBody = attach["replyInfo"]!["text"] + "\n" + attach["replyInfo"]!["replyName"]! + ":" + attach["replyInfo"]!["replyText"]!;
                        mcb.Text(msbBody);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = msbBody;
                    }
                    //礼物回复
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "GIFTREPLY")
                    {
                        msbBody = attach["giftReplyInfo"]!["text"] + "\n" + attach["giftReplyInfo"]!["replyName"]! + ":" + attach["giftReplyInfo"]!["replyText"]!;
                        mcb.Text(msbBody);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = msbBody;
                    }
                    //ֱ直播
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "LIVEPUSH")
                    {
                        msbBody = "直播啦！\n标题：" + attach["livePushInfo"]!["liveTitle"];
                        mcb.Text(msbBody).ImageByUrl(Config.KD.ImgDomain + attach["livePushInfo"]!["liveCover"]!.ToString());
                        if (Config.KD.MsgType.Any(t => t == "AtAll"))
                            mcb.AtAll();
                        saveMsg.Type = messageType;
                        saveMsg.Msg = msbBody;
                    }
                    //语音
                    else if (Config.KD.MsgType.Contains(messageType.ToLower()) && messageType == "AUDIO")
                    {
                        msbBody = attach["audioInfo"]!["url"]!.ToString();
                        mcb.RecordByUrl(msbBody);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = msbBody;
                    }
                    //视频
                    else if (Config.KD.MsgType.Contains(messageType.ToLower()) && messageType == "VIDEO")
                    {
                        msbBody = attach["videoInfo"]!["url"]!.ToString();
                        mcb.VideoByUrl(msbBody);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = msbBody;
                    }
                    // 房间电台
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "TEAM_VOICE")
                    {
                        //判断是否at所有人
                        msbBody = "开启了房间电台";
                        mcb.Text(msbBody);
                        if (Config.KD.MsgType.Any(t => t == "AtAll"))
                            mcb.AtAll();
                        saveMsg.Type = messageType;
                        saveMsg.Msg = msbBody;
                    }
                    //文字翻牌
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "FLIPCARD")
                    {
                        var answer = attach["filpCardInfo"]!["answer"]!.ToString();
                        mcb.Text(answer.ToString());
                        var question = attach["filpCardInfo"]!["question"];
                        mcb.Text("\n粉丝提问" + question);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = question + "：" + answer;
                    }
                    //语音翻牌
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "FLIPCARD_AUDIO")
                    {
                        var answer = JObject.Parse(attach["filpCardInfo"]!["answer"]!.ToString());
                        mcb.RecordByUrl(Config.KD.VideoDomain + answer["url"]);
                        var question = attach["filpCardInfo"]!["question"];
                        mcb.Text("\n粉丝提问" + question);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = question + "：" + answer;
                    }
                    //视频翻牌
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "FLIPCARD_VIDEO")
                    {
                        var answer = JObject.Parse(attach["filpCardInfo"]!["answer"]!.ToString());
                        mcb.Text(Config.KD.VideoDomain + answer["url"]);
                        var question = attach["filpCardInfo"]!["question"];
                        mcb.Text("\n粉丝提问" + question);

                        saveMsg.Type = messageType;
                        saveMsg.Msg = question + "：" + answer;
                    }
                    //表情
                    else if (Config.KD.MsgType.Contains(messageType) && messageType == "EXPRESSIMAGE")
                    {
                        string url = attach["expressImgInfo"]!["emotionRemote"]!.ToString();
                        mcb.ImageByUrl(url);
                        saveMsg.Type = messageType;
                        saveMsg.Msg = url;
                    }
                    else return;
                }
                else return;
                if (Config.KD.SaveMsg == 2)
                {
                    await _pocketMsg.AddAsync(saveMsg);
                }
                else if (Config.KD.SaveMsg == 1 && roleId == 3)
                {
                    await _pocketMsg.AddAsync(saveMsg);
                }
                if (!Config.EnableModule.Bot) return;
                if (roleId != 3) return;
                if (Config.KD.ForwardGroup)
                {
                    var group = string.IsNullOrWhiteSpace(Config.KD.Group) ? Config.QQ.Group : Config.KD.Group;
                    if (!string.IsNullOrWhiteSpace(group))
                    {
                        MsgModel msgModel = new()
                        {
                            MsgChain = mcb.Build(),
                            Ids = group.ToListStr(),
                        };
                        ReciverMsg.AddMsg(msgModel);
                    }
                }
                if (Config.KD.ForwardQQ)
                {
                    MsgModel msgModel = new();
                    msgModel.Type = 2;
                    msgModel.MsgChain = mcb.Build();
                    if (!string.IsNullOrWhiteSpace(Config.KD.QQ))
                    {
                        var qqs = Config.KD.QQ.ToListStr();
                        msgModel.Ids = qqs;
                        ReciverMsg.AddMsg(msgModel);
                    }
                }
                return;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return;
            }
        }
        public async Task LiveMsgReceiver(string str)
        {
            try
            {
                var result = JObject.Parse(str);
                var custom = JObject.Parse(result["custom"]?.ToString() ?? "");
                var roleId = custom["roleId"]?.ToString().ToInt();
                var messageType = result["type"]?.ToString();
                double timeVal = double.Parse(result["time"]?.ToString() ?? "0");
                var time = System.Convert.ToDateTime(DateTime.Parse(DateTime.Now.ToString("1970-01-01 08:00:00")).AddMilliseconds(timeVal).ToString());//����Ϊʱ���ʽ;
                if (roleId != 3) return;
                MessageChainBuild mcb = new();
                mcb.Text($"【{Config.KD.IdolName}|直播间】\n【{time}】\n{result["fromNick"]}:");
                if (messageType == "text")
                    mcb.Text(result["text"]?.ToString() ?? "");
                if (!Config.EnableModule.Bot) return;
                if (Config.KD.ForwardGroup)
                {
                    var group = Config.KD.Group ?? Config.QQ.Group;
                    if (!string.IsNullOrWhiteSpace(group))
                    {
                        MsgModel msgModel = new()
                        {
                            MsgChain = mcb.Build(),
                            Ids = group.ToListStr(),
                        };
                        ReciverMsg.AddMsg(msgModel);
                    }
                }
                if (Config.KD.ForwardQQ)
                {
                    MsgModel msgModel = new();
                    msgModel.Type = 2;
                    msgModel.MsgChain = mcb.Build();
                    if (!string.IsNullOrWhiteSpace(Config.KD.QQ))
                    {
                        var qqs = Config.KD.QQ.ToListStr();
                        msgModel.Ids = qqs;
                        ReciverMsg.AddMsg(msgModel);
                    }
                    else if (!string.IsNullOrWhiteSpace(Config.QQ.Admin))
                    {
                        msgModel.Id = Config.QQ.Admin;
                        ReciverMsg.AddMsg(msgModel);
                    }
                }
                return;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return;
            }
        }

        private string GetIdolName(string serverId)
        {
            var names = Config.KD.IdolName.ToListStr();
            var servers = Config.KD.ServerId.ToListStr();
            var index = servers.IndexOf(serverId);
            if (index == -1) return "未匹配";
            return names[index];
        }

        private static void HandleEmoji(string str, ref MessageChainBuild mcb)
        {
            string pattern = @"\[[^\]]+\]";
            MatchCollection matches = Regex.Matches(str, pattern);
            foreach (Match match in matches.Cast<Match>())
            {
                Console.WriteLine(match.Value);
            }
        }

        private static async Task<string> GetResponse(string url, string? data = null, string? token = null)
        {
            var baseUrl = "https://pocketapi.48.cn";
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl + url),
                Headers =
                    {
                        { "User-Agent", "PocketFans201807/6.0.16 (iPhone; iOS 13.5.1; Scale/2.00)" },
                        { "pa", "d6c1ae7a-5f06-4ef3-bb49-cb3e3c67e8fb" },
                        { "appInfo", "{\"vendor\":\"apple\",\"deviceId\":\"79DWUFH7-GWVM-HQH3-8DNG-ZPNPOTPEFGXW\",\"appVersion\":\"6.2.2\",\"appBuild\":\"21080401\",\"osVersion\":\"11.4.1\",\"osType\":\"ios\",\"deviceName\":\"iPhone XR\",\"os\":\"ios\"}" },
                        { "Accept-Language", "zh-Hans-AW;q=1" },
                        { "Host", "pocketapi.48.cn" },
                    },
            };
            if (token != null)
                request.Headers.Add("Token", token);
            if (data != null)
                request.Content = new StringContent(data)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }
        public static async Task<string> SmsCode(string mobile, string area = "86")
        {
            var data = new JObject()
            {
                {"mobile",mobile},
                {"area",area }
            };
            var res = await GetResponse("/user/api/v1/sms/send2", data.ToString());
            return res;
        }
        public static async Task<string> Login(string mobile, string smsCode)
        {
            var data = new JObject()
            {
                {"mobile",mobile},
                {"code",smsCode }
            };
            var res = await GetResponse("/user/api/v1/login/app/mobile/code", data.ToString());
            return res;
        }
        public static async Task<string> UserInfo(string token)
        {
            var res = await GetResponse("/im/api/v1/im/userinfo", token: token);
            return res;
        }
    }
}