using StarBot.Extension;
using StarBot.IService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StarBot.Extension
{
    public class Baidu
    {
        ISysLog _sysLog;
        ISysConfig _sysConfig;

        public Baidu()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
        }
        public async Task<string> GetBaiduToken()
        {
            var config = await _sysConfig.GetConfig();
            string authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new();
            List<KeyValuePair<string, string>> paraList =
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", config.BD.AppKey),
                new KeyValuePair<string, string>("client_secret", config.BD.AppSeret)
            ];

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
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
                foreach (var item in imageList)
                {
                    string img64 = Base64Helper.PathToBase64(Directory.GetCurrentDirectory() + "/wwwroot" + item);
                    if (string.IsNullOrWhiteSpace(img64)) return 0;
                    string token =await GetBaiduToken();
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
    }
}