using Newtonsoft.Json.Linq;
using System.Globalization;
using Newtonsoft.Json;
using AIdol.IService;
using ShamrockCore.Receiver.MsgChain;
using Config = AIdol.Model.Config;
using AIdol.Extension;
using Microsoft.Extensions.DependencyInjection;
using TBC.CommonLib;

namespace Helper
{
    public class Weibo
    {
        ISysConfig _sysConfig;
        ISysLog _sysLog;
        ISysCache _sysCache;
        public Weibo()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
            _sysCache = factory.GetService<ISysCache>()!;
        }
        public Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }
        private static Dictionary<string, string> GetHeader(string uid)
        {
            return new Dictionary<string, string>()
            {
                {":authority","weibo.com" },
                {":method","GET" },
                {":path","/ajax/statuses/mymblog?uid="+uid },
                {":scheme","https" },
                {"Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
                {"Accept-Language","zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6" },
                {"Cache-Control","no-cache" },
                {"Cookie","XSRF-TOKEN=mgVY3WMp8U-T6Wbu24ifdazm; SUBP=0033WrSXqPxfM72-Ws9jqgMF55529P9D9WhizH8r9Hyn870HzJo4TQoB; SUB=_2AkMSf_1df8NxqwJRmfATxWrlaIV_ywjEieKkIwyGJRMxHRl-yj8XqksbtRB6Of_Tsj1wFglssEkNvyqikP19B0UlIrd8; WBPSESS=NcA3pTjBP9SOtpsXaAXWlx_1aL3IfVadLkk5h-hKiZrhJi_NyNc2r5RbB0ZE0gYuG6ZSJmF8k26JJ46ltyme0fAcMSF9VPonnDU1TPvBjVADJPPa99vi0TVPQDCUKIMU" },
                {"Referer","https://passport.weibo.com/" },
                {"Sec-Ch-Ua","\"Google Chrome\";v=\"117\", \"Not;A=Brand\";v=\"8\", \"Chromium\";v=\"117\"" },
                {"Sec-Ch-Ua-Mobile","?0" },
                {"ec-Ch-Ua-Platform","\"Windows\"" },
                {"Sec-Fetch-Dest","document" },
                {"Sec-Fetch-Mode","navigate" },
                {"Sec-Fetch-Site","same-site" },
                {"Sec-Fetch-User","?1" },
                {"Upgrade-Insecure-Requests","1" },
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36" },
            };
        }
        public async Task Save()
        {
            string url = "";
            try
            {
                var allList = Config.WB.UserAll.ToStrList().Concat(Config.WB.UserPart.ToStrList());
                foreach (var item in allList)
                {
                    url = "https://weibo.com/ajax/statuses/mymblog?uid=" + item;
                    var handler = new HttpClientHandler() { UseCookies = true };
                    HttpClient httpClient = new(handler);
                    var headers = GetHeader(item);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    var res = await httpClient.SendAsync(request);
                    var content = await res.Content.ReadAsStringAsync();
                    var data = JObject.Parse(content.Replace("&lt;", ""));
                    var list = JArray.FromObject(data["data"]!["list"]!);
                    foreach (JObject blog in list.Cast<JObject>())
                    {
                        DateTime createDate = new();
                        CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                        string format = "ddd MMM d HH:mm:ss zz00 yyyy";
                        string stringValue = blog!["created_at"]!.ToString();
                        if (!string.IsNullOrWhiteSpace(stringValue))
                            createDate = DateTime.ParseExact(stringValue, format, cultureInfo); // 将字符串转换成日期
                        if (createDate >= DateTime.Now.AddMinutes(-Config.WB.TimeSpan))
                        {
                            //可以认定为新发的微博
                            //获取微博类型0-视频，2-图文,1-转发微博
                            var mblogtype = -1;
                            if (blog.ContainsKey("page_info")) mblogtype = 0;
                            if (blog.ContainsKey("pic_infos")) mblogtype = 2;
                            if (blog.ContainsKey("topic_struct")) mblogtype = 1;
                            //需要发送通知则发送通知
                            if (Config.WB.UserAll.Contains(item))
                            {
                                var msgModel = new MsgModel();
                                var mcb = new MessageChainBuilder();
                                if (mblogtype == 2)
                                {
                                    //获取第一张图片发送
                                    var first = blog["pic_infos"]![JArray.FromObject(blog["pic_ids"]!)[0]!.ToString()]!["large"]!["url"]!.ToString();
                                    mcb.Text($"{blog["user"]!["screen_name"]}发微博啦！\n");
                                    mcb.Text(blog["text_raw"]!.ToString()).ImageByUrl(first);

                                }
                                else if (mblogtype == 0)
                                {
                                    mcb.Text($"{blog["user"]!["screen_name"]}发微博啦！\n");
                                    var pageInfo = (JObject?)blog["page_info"];
                                    if (pageInfo != null)
                                    {
                                        var objType = pageInfo["object_type"]!.ToString();
                                        if (objType == "video")
                                        {
                                            mcb.Text(blog["text_raw"]!.ToString());
                                            mcb.Text("视频链接：" + pageInfo["media_info"]!["h5_url"]!);
                                        }
                                    }
                                }
                                else if (mblogtype == 1)
                                {
                                    mcb.Text($"{blog["user"]!["screen_name"]}转发了微博\n");
                                    mcb.Text(blog["text_raw"]!.ToString());
                                }
                                else
                                {
                                    mcb.Text($"{blog["user"]!["screen_name"]}发微博啦！\n");
                                    mcb.Text(blog["text_raw"]?.ToString() ?? "");
                                }
                                mcb.Text($"\n链接：https://m.weibo.cn/status/{blog["mid"]}");
                                if (Config.WB.ForwardGroup)
                                {
                                    var goups = Config.WB.Group ?? Config.QQ.Group;
                                    if (goups == null) continue;
                                    await ReciverMsg.Instance.SendGroupMsg(goups.ToStrList(), mcb.Build());
                                }
                                if (Config.WB.ForwardQQ)
                                {
                                    if (string.IsNullOrWhiteSpace(Config.WB.QQ)) continue;
                                    await ReciverMsg.Instance.SendFriendMsg(Config.WB.QQ, mcb.Build());
                                }
                            }
                            //保存图片
                            if (mblogtype == 2)
                            {
                                var picList = blog["pic_ids"]!.Select(t => t.ToString()).ToList();
                                if (picList == null) continue;
                                foreach (var picId in picList)
                                {
                                    var picInfo = (JObject)blog["pic_infos"]!;
                                    if (picInfo.ContainsKey(picId))
                                    {
                                        var imgUrl = picInfo[picId]!["original"]!["url"]!.ToString();
                                        var fileName = Path.GetFileName(imgUrl);
                                        imgUrl = "https://cdn.ipfsscan.io/weibo/large/" + fileName;
                                        await FatchFace(imgUrl, true);
                                    }
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

        public async Task ChiGua()
        {
            try
            {
                foreach (var item in Config.WB.ChiGuaUser.ToStrList())
                {
                    var url = "https://weibo.com/ajax/statuses/mymblog?uid=" + item;
                    HttpClient httpClient = new();
                    var headers = GetHeader(item);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    var res = await httpClient.SendAsync(request);
                    var content = await res.Content.ReadAsStringAsync();
                    var data = JObject.Parse(content);
                    var list = JArray.FromObject(data["data"]!["list"]!);
                    foreach (JObject blog in list)
                    {
                        DateTime createDate = new();
                        CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                        string format = "ddd MMM d HH:mm:ss zz00 yyyy";
                        string stringValue = blog!["created_at"]!.ToString();
                        if (!string.IsNullOrWhiteSpace(stringValue))
                            createDate = DateTime.ParseExact(stringValue, format, cultureInfo); // 将字符串转换成日期
                        if (createDate >= DateTime.Now.AddMinutes(-Config.WB.TimeSpan))
                        {
                            //可以认定为新发的微博
                            //获取微博类型0-视频，2-图文
                            var mblogtype = -1;
                            if (blog.ContainsKey("page_info")) mblogtype = 0;
                            if (blog.ContainsKey("pic_infos")) mblogtype = 2;
                            var blogContent = blog["text_raw"]!.ToString();
                            if (!Config.WB.Keyword.ToStrList().Select(blogContent.Contains).Any(x => x) && Config.WB.Keyword.ToStrList().Count > 0)
                                return;
                            var mcb = new MessageChainBuilder();
                            var msgModel = new MsgModel { MsgStr = $"{blog["user"]!["screen_name"]}发了一条相关微博！" + $"\n链接：https://weibo.com/{blog["user"]!["id"]}/{blog["mid"]}\n" };
                            mcb.Text(msgModel.MsgStr);
                            if (mblogtype == 2)
                            {
                                //获取第一张图片发送
                                var first = blog["pic_infos"]![JArray.FromObject(blog["pic_ids"]!)[0]!.ToString()]!["large"]!["url"]!.ToString();
                                mcb.Text(blog["text_raw"]!.ToString()).ImageByUrl(first);
                            }
                            else if (mblogtype == 0)
                            {
                                var pageInfo = (JObject?)blog["page_info"];
                                if (pageInfo != null)
                                {
                                    var objType = pageInfo["object_type"]!.ToString();
                                    if (objType == "video")
                                    {
                                        msgModel.Type = 0;
                                        mcb.Text(blog["text_raw"]!.ToString()).Text("视频链接：" + pageInfo["media_info"]!["h5_url"]!);
                                    }
                                }
                            }
                            else
                                mcb.Text(blog["text_raw"]?.ToString() ?? "");
                            mcb.Text($"\n链接：https://m.weibo.cn/status/{blog["mid"]}");
                            //需要发送通知则发送通知
                            if (Config.WB.ForwardGroup)
                            {
                                var goups = Config.WB.Group ?? Config.QQ.Group;
                                if (goups == null) continue;
                                await ReciverMsg.Instance.SendGroupMsg(goups.ToStrList(), mcb.Build());
                            }
                            if (Config.WB.ForwardQQ)
                            {
                                if (string.IsNullOrWhiteSpace(Config.WB.QQ)) continue;
                                await ReciverMsg.Instance.SendFriendMsg(Config.WB.QQ, mcb.Build());
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

        public async Task SaveByUrl(string id)
        {
            try
            {
                await ReciverMsg.Instance.SendAdminMsg("开始识别微博连接");
                var url = "https://weibo.com/ajax/statuses/show?id=" + id;
                var handler = new HttpClientHandler() { UseCookies = true };
                HttpClient httpClient = new(handler);
                var headers = GetHeader(id);
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                var res = await httpClient.SendAsync(request);
                var content = await res.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return;
                var obj = JObject.Parse(content);
                var picsStr = obj["pic_ids"]!;
                var picList = JsonConvert.DeserializeObject<List<string>>(picsStr.ToString())!;
                await ReciverMsg.Instance.SendAdminMsg($"检测到有{picList.Count}张图！开始进行识别保存！");
                foreach (var item in picList)
                {
                    var img = "https://cdn.ipfsscan.io/weibo/large/" + item + ".jpg";
                    await FatchFace(img, true);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
            }
        }

        public async Task FatchFace(string url, bool save = false)
        {
            try
            {
                if (!Config.EnableModule.BD)
                {
                    await _sysCache.AddAsync(new()
                    {
                        Content = url,
                        Type = 1
                    });
                    await ReciverMsg.Instance.SendAdminMsg($"未启用人脸识别，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                    return;
                }
                var face = await new Baidu().IsFaceAndCount(url);
                if (face == 1)
                {
                    var score = await new Baidu().FaceMatch(url);
                    if (score != Config.BD.Audit) await ReciverMsg.Instance.SendAdminMsg($"人脸对比相似度：{score}");

                    if (score >= Config.BD.Audit && score < Config.BD.Similarity)
                    {
                        await _sysCache.AddAsync(new()
                        {
                            Content = url,
                            Type = 1
                        });
                        await ReciverMsg.Instance.SendAdminMsg($"相似度低于{Config.BD.Similarity}，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                        return;
                    }
                    if (score >= Config.BD.Similarity && score <= 100)
                    {
                        if (!await new FileHelper().Save(url))
                        {
                            await _sysCache.AddAsync(new()
                            {
                                Content = url,
                                Type = 1
                            });
                            await ReciverMsg.Instance.SendAdminMsg($"保存失败，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                        }
                        else
                        {
                            string msg = $"相似大于{Config.BD.Similarity}，已保存本地";
                            if (Config.BD.SaveAliyunDisk) msg += $"，正在上传至阿里云盘【{Config.BD.AlbumName}】相册";
                            await ReciverMsg.Instance.SendAdminMsg(msg);
                        }
                        return;
                    }
                    if (save)
                    {
                        await _sysCache.AddAsync(new()
                        {
                            Content = url,
                            Type = 1
                        });
                        await ReciverMsg.Instance.SendAdminMsg($"加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                        return;
                    }
                }
                else if (face > 1)
                {
                    await _sysCache.AddAsync(new()
                    {
                        Content = url,
                        Type = 1
                    });
                    await ReciverMsg.Instance.SendAdminMsg($"识别到多个人脸，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                    return;
                }
                else if (face == 0 && save)
                {
                    await _sysCache.AddAsync(new()
                    {
                        Content = url,
                        Type = 1
                    });
                    await ReciverMsg.Instance.SendAdminMsg($"未识别到人脸，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                }
                return;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return;
            }
        }
    }
}
