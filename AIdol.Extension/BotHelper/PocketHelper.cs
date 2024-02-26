using AIdol.Extension;
using AIdol.IService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using ShamrockCore.Receiver.MsgChain;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TBC.CommonLib;
using Config = AIdol.Model.Config;

namespace Helper
{
    public class Pocket
    {
        private static Pocket? instance;
        private static readonly object lockObject = new object();
        ISysConfig _sysConfig;
        ISysLog _sysLog;

        private Pocket()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
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

                if (roleId != 3) return;
                #region �����߼�
                else
                {
                    MessageChainBuilder mcb = new();
                    mcb.Text($"��{Config.KD.IdolName}|{channelName}��\n��{time}��\n{name}:");
                    //ͼƬ
                    if (msgType == "image")
                    {
                        msbBody = result["attach"]!["url"]!.ToString();
                        await Task.Run(async () =>
                        {
                            await new Weibo().FatchFace(msbBody);
                        });
                        if (!Config.KD.MsgType.Contains(msgType)) return;
                        mcb.ImageByUrl(msbBody);
                    }
                    //����
                    else if (Config.KD.MsgType.Contains(msgType) && msgType == "text")
                    {
                        //"230226137"
                        msbBody = result["body"]!.ToString();
                        mcb.Text(msbBody);
                    }
                    //��Ƶ
                    else if (Config.KD.MsgType.Contains(msgType) && msgType == "video")
                    {
                        mcb.Text(result["attach"]!["url"]!.ToString());
                    }
                    //����
                    else if (Config.KD.MsgType.Contains(msgType) && msgType == "audio")
                    {
                        mcb.RecordByUrl(result["attach"]!["url"]!.ToString());
                    }
                    else if (msgType == "custom")
                    {
                        var attach = result["attach"]!;
                        var messageType = attach["messageType"]!.ToString();
                        //�ظ�
                        if (Config.KD.MsgType.Contains(messageType) && messageType == "REPLY")
                        {
                            msbBody = attach["replyInfo"]!["text"] + "\n" + attach["replyInfo"]!["replyName"]! + ":" + attach["replyInfo"]!["replyText"]!;
                            mcb.Text(msbBody);
                        }
                        //����ظ�
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "GIFTREPLY")
                        {
                            msbBody = attach["giftReplyInfo"]!["text"] + "\n" + attach["giftReplyInfo"]!["replyName"]! + ":" + attach["giftReplyInfo"]!["replyText"]!;
                            mcb.Text(msbBody);
                        }
                        //��ѡ�Ʒ�
                        //else if (false)
                        //{
                        //    msbBody = "�ͳ��ˡ�" + attach["giftInfo"]!["giftName"] + "��" + attach["giftInfo"]!["tpNum"] + "�֣�����";
                        //    mcb.Text(msbBody);
                        //}
                        //ֱ��
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "LIVEPUSH")
                        {
                            //�ж��Ƿ�at������
                            msbBody = "ֱ������\n���⣺" + attach["livePushInfo"]!["liveTitle"];
                            mcb.Text(msbBody).ImageByUrl(Config.KD.ImgDomain + attach["livePushInfo"]!["liveCover"]!.ToString());
                            if (Config.KD.MsgType.FirstOrDefault(t => t == "AtAll")?.ToBool() ?? false)
                                mcb.AtAll();
                        }
                        //����
                        else if (Config.KD.MsgType.Contains(messageType.ToLower()) && messageType == "AUDIO")
                        {
                            mcb.RecordByUrl(attach["audioInfo"]!["url"]!.ToString());
                        }
                        //��Ƶ
                        else if (Config.KD.MsgType.Contains(messageType.ToLower()) && messageType == "VIDEO")
                        {
                            mcb.VideoByUrl(attach["videoInfo"]!["url"]!.ToString());
                        }
                        // �����̨
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "TEAM_VOICE")
                        {
                            //�ж��Ƿ�at������
                            msbBody = "�����˷����̨";
                            mcb.Text(msbBody);
                            if (Config.KD.MsgType.FirstOrDefault(t => t == "AtAll")?.ToBool() ?? false)
                                mcb.AtAll();
                        }
                        //���ַ���
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "FLIPCARD")
                        {
                            var answer = attach["filpCardInfo"]!["answer"]!.ToString();
                            mcb.Text(answer.ToString());
                            mcb.Text("\n��˿���ʣ�" + attach["filpCardInfo"]!["question"]);
                        }
                        //��������
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "FLIPCARD_AUDIO")
                        {
                            var answer = JObject.Parse(attach["filpCardInfo"]!["answer"]!.ToString());
                            mcb.RecordByUrl(Config.KD.VideoDomain + answer["url"]);
                            mcb.Text("\n��˿���ʣ�" + attach["filpCardInfo"]!["question"]);
                        }
                        //��Ƶ����
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "FLIPCARD_VIDEO")
                        {
                            var answer = JObject.Parse(attach["filpCardInfo"]!["answer"]!.ToString());
                            mcb.Text(Config.KD.VideoDomain + answer["url"]);
                            mcb.Text("\n��˿���ʣ�" + attach["filpCardInfo"]!["question"]);
                        }
                        //����
                        else if (Config.KD.MsgType.Contains(messageType) && messageType == "EXPRESSIMAGE")
                        {
                            string url = attach["expressImgInfo"]!["emotionRemote"]!.ToString();
                            mcb.ImageByUrl(url);
                        }
                        else return;
                    }
                    else return;
                    if (!Config.Shamrock.Use) return;
                    if (Config.KD.ForwardGroup)
                    {
                        var group = Config.KD.Group.Count > 0 ? Config.KD.Group : Config.QQ.Group;
                        if (group.Count > 0)
                        {
                            MsgModel msgModel = new()
                            {
                                MsgChain = mcb.Build(),
                                Ids = group,
                            };
                            ReciverMsg.AddMsg(msgModel);
                        }
                    }
                    if (Config.KD.ForwardQQ)
                    {
                        MsgModel msgModel = new();
                        msgModel.Type = 2;
                        msgModel.MsgChain = mcb.Build();
                        if (Config.KD.QQ.Count > 0)
                        {
                            var qqs = Config.KD.QQ;
                            msgModel.Ids = qqs;
                            ReciverMsg.AddMsg(msgModel);
                        }
                        else if (!string.IsNullOrWhiteSpace(Config.QQ.Admin))
                        {
                            msgModel.Id = Config.QQ.Admin;
                            ReciverMsg.AddMsg(msgModel);
                        }
                    }
                }
                #endregion
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
                MessageChainBuilder mcb = new();
                mcb.Text($"��{Config.KD.IdolName}|ֱ���䡿\n��{time}��\n{result["fromNick"]}:");
                if (messageType == "text")
                    mcb.Text(result["text"]?.ToString() ?? "");
                if (!Config.Shamrock.Use) return;
                if (Config.KD.ForwardGroup)
                {
                    var group = Config.KD.Group.Count > 0 ? Config.KD.Group : Config.QQ.Group;
                    if (group.Count > 0)
                    {
                        MsgModel msgModel = new()
                        {
                            MsgChain = mcb.Build(),
                            Ids = group,
                        };
                        ReciverMsg.AddMsg(msgModel);
                    }
                }
                if (Config.KD.ForwardQQ)
                {
                    MsgModel msgModel = new();
                    msgModel.Type = 2;
                    msgModel.MsgChain = mcb.Build();
                    if (Config.KD.QQ.Count > 0)
                    {
                        var qqs = Config.KD.QQ;
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

        private static void HandleEmoji(string str, ref MessageChainBuilder mcb)
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