using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginServer;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Text;
using System.Xml;
using UnifyBot.Receiver.MessageReceiver;

namespace Plugins
{
    public class ChatGPT : BasePlugin
    {

        public static Dictionary<string, List<object>> LastMsg = new();
        public string SecretKey
        {
            get
            {
                XmlDocument doc = new();
                doc.Load(Config);
                var node = doc.SelectSingleNode("SecretKey");
                if (node != null)
                    if (!node.InnerText.Contains("key值"))
                        return node.InnerText;
                return "";
            }
        }
        public override string Name { get; set; } = "ChatGPT";
        public override string Desc { get; set; } = "免费的ChatGPT插件";
        public override string Version { get; set; } = "0.0.3";
        private string Config => ConfPath + "ChatGPT.xml";
        private string Log => LogPath + "ChatGPT.txt";
        public ChatGPT()
        {
            try
            {
                if (!File.Exists(Config))
                {
                    // 创建一个新的 XmlDocument
                    XmlDocument doc = new();

                    // 创建 XML 声明
                    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    doc.AppendChild(xmlDeclaration);

                    // 创建 Secrets 节点
                    XmlElement secretsNode = doc.CreateElement("SecretKey");
                    secretsNode.InnerText = "key值填入这，填入前请删除内容";
                    doc.AppendChild(secretsNode);

                    // 创建注释
                    XmlComment comment = doc.CreateComment("请到（https://github.com/chatanywhere/GPT_API_free）申请内测免费key后填入SecretKey中");

                    // 将注释添加到 Secrets 节点之前
                    secretsNode.ParentNode!.InsertBefore(comment, secretsNode);

                    doc.Save(Config);
                }
            }
            catch (Exception e)
            {
                File.AppendAllLines(Log, [e.Message]);
                return;
            }
        }
        public override async Task FriendMessage(PrivateReceiver gmr)
        {
            var text = gmr.Message?.GetPlainText();
            if (text?.Contains("#GPT ") ?? false)
            {
                var question = text.Replace("#GPT ", "");
                if (gmr.Sender != null)
                {
                    var answer = await GetAnswer(question, gmr.Sender.QQ.ToString());
                    await gmr.SendMessage(answer);
                }
            }
        }

        public async Task<string> GetAnswer(string question, string qq)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SecretKey)) return "请配置密钥";
                if (string.IsNullOrWhiteSpace(question)) return "请输入问题！";
                var url = "https://api.chatanywhere.com.cn/v1/chat/completions";
                var objs = new List<object>();
                if (LastMsg.TryGetValue(qq, out List<object>? value)) objs.AddRange(value);
                objs.Add(new
                {
                    role = "user",
                    content = question
                });
                var obj = new
                {
                    model = "gpt-3.5-turbo",
                    messages = objs
                };
                var body = JsonConvert.SerializeObject(obj);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new StringContent(body)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SecretKey}");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Apifox/1.0.0 (https://apifox.com)");

                var response = await client.SendAsync(request);
                var res = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(res);
                StringBuilder str = new();
                if (data.ContainsKey("choices"))
                {
                    if (data["choices"] != null)
                    {
                        var list = JArray.FromObject(data["choices"]!);
                        if (list.Count > 0)
                        {
                            foreach (JObject item in list.Cast<JObject>())
                            {
                                str.Append(item["message"]!["content"]!.ToString());
                                if (LastMsg.ContainsKey(qq))
                                {
                                    if (LastMsg[qq].Count > 10) LastMsg[qq].Clear();
                                    LastMsg[qq].Add(new
                                    {
                                        role = "assistant",
                                        content = item["message"]!["content"]
                                    });
                                }
                                else
                                {
                                    LastMsg.Add(qq, []);
                                    LastMsg[qq].Add(new
                                    {
                                        role = "assistant",
                                        content = item["message"]!["content"]
                                    });
                                }
                            }
                        }
                    }
                }
                return str.ToString();
            }
            catch (Exception e)
            {
                await File.AppendAllLinesAsync(Log, [e.Message]);
                return "";
            }
        }
    }
}
