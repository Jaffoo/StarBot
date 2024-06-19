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
                    await ReciverMsg.Instance.SendAdminMsg($"����{resource}��ͼƬ��δ��������ʶ���ܣ��������ˣ�Ŀǰ��{ReciverMsg.Instance.Check.Count}��ͼƬ�����");
                    return;
                }
                var face = await new Baidu().IsFaceAndCount(url);
                if (face == 1)
                {
                    var score = await new Baidu().FaceMatch(url);
                    if (score != Config.BD.Audit) await ReciverMsg.Instance.SendAdminMsg($"�����Ա����ƶȣ�{score}");

                    if (score >= Config.BD.Audit && score < Config.BD.Similarity)
                    {
                        await _sysCache.AddAsync(new()
                        {
                            Content = url,
                            Type = 1,
                            CreateDate = DateTime.Now,
                        });
                        await ReciverMsg.Instance.SendAdminMsg($"����{resource}��ͼƬ�����ƶȵ���{Config.BD.Similarity}���������ˣ�Ŀǰ��{ReciverMsg.Instance.Check.Count}��ͼƬ�����");
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
                            await ReciverMsg.Instance.SendAdminMsg($"����{resource}��ͼƬ����ʧ�ܣ��������ˣ�Ŀǰ��{ReciverMsg.Instance.Check.Count}��ͼƬ�����");
                        }
                        else
                        {
                            string msg = $"���ƴ���{Config.BD.Similarity}���ѱ��汾��";
                            if (Config.BD.SaveAliyunDisk) msg += $"�������ϴ����������̡�{Config.BD.AlbumName}�����";
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
                    await ReciverMsg.Instance.SendAdminMsg($"����{resource}��ͼƬ��ʶ�𵽶���������������ˣ�Ŀǰ��{ReciverMsg.Instance.Check.Count}��ͼƬ�����");
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
                    await ReciverMsg.Instance.SendAdminMsg($"δʶ���������������ˣ�Ŀǰ��{ReciverMsg.Instance.Check.Count}��ͼƬ�����");
                }
                return;
            }
            catch (Exception e)
            {
                await _sysLog.WriteLog("(ERROR_4)����ʶ��ʧ�ܣ�");
                UtilHelper.WriteLog(e.Message, prefix: "ERROR_4");
                return;
            }
        }
    }
}