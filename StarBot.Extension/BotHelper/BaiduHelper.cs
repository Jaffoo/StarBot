using StarBot.IService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StarBot.Helper;
using StarBot.Model;

namespace StarBot.Extension
{
    public class Baidu
    {
        ISysLog _sysLog;
        ISysConfig _sysConfig;
        ISysCache _sysCache;

        public Baidu()
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

        public async Task<string> GetBaiduToken()
        {
            string authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new();
            List<KeyValuePair<string, string>> paraList =
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", Config.BD.AppKey),
                new KeyValuePair<string, string>("client_secret", Config.BD.AppSeret)
            ];

            HttpResponseMessage response = await client.PostAsync(authHost, new FormUrlEncodedContent(paraList));
            string result = response.Content.ReadAsStringAsync().Result;
            var token = JObject.Parse(result)["access_token"]?.ToString() ?? "";
            return token;
        }

        public async Task<int> IsFaceAndCount(string img)
        {
            try
            {
                img = await Base64Helper.UrlImgToBase64(img);
                if (string.IsNullOrWhiteSpace(img)) return 0;
                string token = await GetBaiduToken();
                string host = "https://aip.baidubce.com/rest/2.0/face/v3/detect?access_token=" + token;
                string str = "{\"image\":\"" + img + "\",\"image_type\":\"BASE64\",\"face_type\":\"LIVE\",\"liveness_control\":\"LOW\"}";
                StringContent stringContent = new(str);
                HttpClient client = new();
                var response = await client.PostAsync(host, stringContent);
                var res = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(res);
                if (string.IsNullOrWhiteSpace(result["result"]?.ToString())) return 0;
                var obj = JObject.Parse(JsonConvert.SerializeObject(result["result"]));
                if (!obj.ContainsKey("face_num")) return 0;
                var faceNum = Convert.ToInt32(obj["face_num"]?.ToString() ?? "0");
                return faceNum;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return 0;
            }
        }

        public async Task<float> FaceMatch(string img)
        {
            try
            {
                var config = await _sysConfig.GetConfig();
                img = await Base64Helper.UrlImgToBase64(img);
                if (string.IsNullOrWhiteSpace(img)) return 0;
                var imageList = config.BD.ImageList;
                List<float> scores = [];
                string img64 = string.Empty;
                foreach (var item in imageList)
                {
                    if (item.Url.Contains("http"))
                        img64 = await Base64Helper.UrlImgToBase64(item.Url);
                    else
                        img64 = Base64Helper.PathToBase64("wwwroot" + item.Url);
                    if (string.IsNullOrWhiteSpace(img64)) return 0;
                    string token = await GetBaiduToken();
                    string host = "https://aip.baidubce.com/rest/2.0/face/v3/match?access_token=" + token;

                    string str = "[{\"image\": \"" + img + "\", \"image_type\": \"BASE64\", \"face_type\": \"LIVE\", \"quality_control\": \"LOW\"}," +
         "{\"image\": \"" + img64 + "\", \"image_type\": \"BASE64\", \"face_type\": \"LIVE\", \"quality_control\": \"LOW\"}]";
                    StringContent stringContent = new(str);
                    HttpClient client = new();
                    var response = await client.PostAsync(host, stringContent);
                    var resultStr = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(resultStr);
                    if (string.IsNullOrWhiteSpace(result["result"]?.ToString() ?? "")) continue;
                    if (string.IsNullOrWhiteSpace(result["result"]?.ToString() ?? "")) return 0;
                    var obj = (JObject)result["result"]!;
                    if (obj == null) return 0;
                    var score = float.Parse(obj["score"]?.ToString() ?? "0");
                    scores.Add(score);
                    await Task.Delay(1000);
                }
                return scores.Count == 0 ? 0 : scores.Max();
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog(e.Message);
                return 0;
            }
        }

        public async Task FatchFace(string url, string resource = "")
        {
            try
            {
                if (!Config.EnableModule.BD || !Config.BD.FaceVerify)
                {
                    await _sysCache.AddAsync(new()
                    {
                        Content = url,
                        Type = 1,
                        CreateDate = DateTime.Now,
                    });
                    await ReciverMsg.Instance.SendAdminMsg($"来自{resource}的图片，未启用人脸识别功能，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
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
                            Type = 1,
                            CreateDate = DateTime.Now,
                        });
                        await ReciverMsg.Instance.SendAdminMsg($"来自{resource}的图片，相似度低于{Config.BD.Similarity}，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                        return;
                    }
                    if (score >= Config.BD.Similarity && score <= 100)
                    {
                        if (!await new FileHelper().Save(url))
                        {
                            await _sysCache.AddAsync(new()
                            {
                                Content = url,
                                Type = 1,
                                CreateDate = DateTime.Now,
                            });
                            await ReciverMsg.Instance.SendAdminMsg($"来自{resource}的图片保存失败，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                        }
                        else
                        {
                            string msg = $"相似大于{Config.BD.Similarity}，已保存本地";
                            if (Config.BD.SaveAliyunDisk) msg += $"，正在上传至阿里云盘【{Config.BD.AlbumName}】相册";
                            await ReciverMsg.Instance.SendAdminMsg(msg);
                        }
                        return;
                    }
                }
                else if (face > 1)
                {
                    await _sysCache.AddAsync(new()
                    {
                        Content = url,
                        Type = 1,
                        CreateDate = DateTime.Now,
                    });
                    await ReciverMsg.Instance.SendAdminMsg($"来自{resource}的图片，识别到多个人脸，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                    return;
                }
                else if (face == 0)
                {
                    await _sysCache.AddAsync(new()
                    {
                        Content = url,
                        Type = 1,
                        CreateDate = DateTime.Now,
                    });
                    await ReciverMsg.Instance.SendAdminMsg($"未识别到人脸，加入待审核，目前有{ReciverMsg.Instance.Check.Count}张图片待审核");
                }
                return;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog("(ERROR_4)人脸识别失败！");
                UtilHelper.WriteLog(e.Message, prefix: "ERROR_4");
                return;
            }
        }
    }
}