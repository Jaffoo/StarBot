using HtmlAgilityPack;
using IdolBot.Extension;
using IdolBot.IService;
using IdolBot.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using ShamrockCore.Receiver.MsgChain;
using System.IO.Compression;
using System.Text;
using Config = IdolBot.Model.Config;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Helper
{
    public class Xhs
    {
        ISysConfig _sysConfig;
        ISysLog _sysLog;
        public Xhs()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
        }
        private Config Config
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
                {":authority","www.xiaohongshu.com" },
                {":method","GET" },
                {":path","/user/profile/"+uid },
                {":scheme","https" },
                {"Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
                {"Accept-Encoding","gzip, deflate, br" },
                {"Accept-Language","zh-CN,zh;q=0.9" },
                {"Cache-Control","max-age=0" },
                {"Cookie","abRequestId=7d588f80-219d-5227-b1e8-9ef14746e504; webBuild=3.15.5; xsecappid=xhs-pc-web; a1=18bc6ce776fhc0tdaj76tjpj4ricat1yboom8h2ly50000366008; webId=3952f67d2805670c7109d0af98449541; gid=yYDSKSdj4fDSyYDSKSdWWlxJKixS89f0VWK9l1vVUV4F3628S09yvY888qKK88Y8SKKYJyd8; web_session=030037a262c9216f033c19db29224a9b9d681f" },
                {"Sec-Ch-Ua","\"Chromium\";v=\"118\", \"Google Chrome\";v=\"118\", \"Not=A?Brand\";v=\"99\"" },
                {"Sec-Ch-Ua-Mobile","?0" },
                {"ec-Ch-Ua-Platform","\"Windows\"" },
                {"Sec-Fetch-Dest","document" },
                {"Sec-Fetch-Mode","navigate" },
                {"Sec-Fetch-Site","same-origin" },
                {"Sec-Fetch-User","?1" },
                {"Upgrade-Insecure-Requests","1" },
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36" },
            };
        }

        public async Task Save()
        {
            string url = "";
            try
            {
                foreach (var item in Config.XHS.User)
                {
                    url = "https://www.xiaohongshu.com/user/profile/" + item;
                    var handler = new HttpClientHandler() { UseCookies = true };
                    HttpClient httpClient = new(handler);
                    var headers = GetHeader(item);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    var res = await httpClient.SendAsync(request);
                    var stream = await res.Content.ReadAsStreamAsync();
                    var html = Encoding.UTF8.GetString(Decompress(stream));
                    HtmlDocument doc = new();
                    doc.LoadHtml(html);
                    HtmlNode? scriptNode = doc.DocumentNode.Descendants("script")
                    .FirstOrDefault(n => n.InnerText.Contains("window.__INITIAL_STATE__"));
                    if (scriptNode == null) continue;
                    var text = scriptNode.InnerText;
                    var jsonObj = text.Replace("window.__INITIAL_STATE__=", "");
                    JObject obj = JObject.Parse(jsonObj);
                    var notesStr = obj["user"]?["notes"]?[0]?.ToString();
                    if (string.IsNullOrWhiteSpace(notesStr)) continue;
                    var notes = JArray.Parse(notesStr);
                    if (notes == null) continue;
                    var model = notes.Where(t => t["noteCard"]!["interactInfo"]!["sticky"]!.Value<bool>() != true).FirstOrDefault();
                    if (model == null) continue;
                    var noteUrl = "https://www.xiaohongshu.com/explore/" + model["id"];
                    request = new HttpRequestMessage(HttpMethod.Get, noteUrl);
                    res = await httpClient.SendAsync(request);
                    html = await res.Content.ReadAsStringAsync();
                    doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    scriptNode = doc.DocumentNode.Descendants("script")
                   .FirstOrDefault(n => n.InnerText.Contains("window.__INITIAL_STATE__"));
                    if (scriptNode == null) continue;
                    text = scriptNode.InnerText;
                    jsonObj = text.Replace("window.__INITIAL_STATE__=", "");
                    obj = JObject.Parse(jsonObj);
                    var noteStr = obj["note"]!["noteDetailMap"]![model["id"]!.ToString()]!["note"]!.ToString();
                    if (string.IsNullOrWhiteSpace(noteStr)) continue;
                    var note = JObject.Parse(noteStr);
                    var timestamp = Convert.ToInt64(note["time"]!);
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                    // 转换为本地时间
                    DateTime createDate = dateTimeOffset.LocalDateTime;
                    if (createDate < DateTime.Now.AddMinutes(-Config.XHS.TimeSpan)) continue;
                    //可以认定为是新发的笔记
                    //type=1-图片，type=2-视频
                    var type = note["type"]!.ToString() == "normal" ? 1 : 2;
                    var first = "";
                    if (type == 1)
                    {
                        //图片
                        var imageList = JArray.Parse(note["imageList"]!.ToString());
                        first = imageList[0]!["infoList"]?[1]?["url"]?.ToString();
                        //保存阿里云盘
                        foreach (var pic in imageList)
                        {
                            var imgUrl = pic["infoList"]?[1]?["url"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(imgUrl))
                                await new Weibo().FatchFace(imgUrl, true);
                        }
                    }
                    else
                    {
                        //视频
                    }
                    //发送消息
                    var str = $"{note["user"]!["nickname"]!}发小红书啦！\n类型：图片\n标题：{note["title"]}\n内容：{note["desc"]}";
                    var mcb = new MessageChainBuilder();
                    mcb.Text(noteUrl).Text(str);
                    if (type == 1 && !string.IsNullOrWhiteSpace(first)) mcb.ImageByUrl(first);
                    if (Config.XHS.ForwardGroup)
                        foreach (var group in Config.XHS.Group)
                            await ReciverMsg.Instance.SendGroupMsg(group, mcb.Build());
                    if (Config.XHS.ForwardQQ)
                        foreach (var qq in Config.XHS.QQ)
                            await ReciverMsg.Instance.SendFriendMsg(qq, mcb.Build());
                }
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }
        public static byte[] Decompress(Stream stream)
        {
            var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            var mmStream = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int byteRead = gzipStream.Read(block, 0, block.Length);
                if (byteRead <= 0)
                    break;
                else
                    mmStream.Write(block, 0, byteRead);
            }
            mmStream.Close();
            return mmStream.ToArray();
        }
    }
}