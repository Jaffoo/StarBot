using StarBot.Entity;
using StarBot.Extension;
using StarBot.IService;
using StarBot.Model;
using StarBot.Timer;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using UnifyBot.Model;
using System.Diagnostics;
using TBC.CommonLib;
using Newtonsoft.Json.Linq;
using SqlSugar.Extensions;
using System.Text;
using System.Net;

namespace StarBot.Controllers
{
    public class HomeController(ISysConfig sysConfig, ISysCache sysCache, ISysIdol sysIdol, ISysLog sysLog, IWebHostEnvironment env) : BaseController
    {
        private readonly IWebHostEnvironment _env = env;
        ISysConfig _sysConfig = sysConfig;
        ISysCache _sysCache = sysCache;
        ISysIdol _sysIdol = sysIdol;
        ISysLog _sysLog = sysLog;
        public static Process? AliProcess = null;
        private static UnifyBot.Bot? TempBot = null;
        public Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }

        /// <summary>
        /// 判断api服务是否启动
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        public ApiResult Index()
        {
            var environment = "development";
            if (_env.IsProduction()) environment = "production";
            return Success("Environment:" + environment);
        }

        /// <summary>
        /// 启动qq机器人
        /// </summary>
        /// <returns></returns>
        [HttpGet("startbot")]
        public ApiResult StartBot(bool botReady = true)
        {
            try
            {
                if (botReady)
                    JobManager.Initialize(new FluentSchedulerFactory());
                else
                    JobManager.RemoveAllJobs();
                if (Config.EnableModule.Bot)
                {
                    if (botReady)
                    {
                        var connectConfig = new Connect(Config.Bot.Host, Config.Bot.WebsocktPort, Config.Bot.HttpPort, token: Config.Bot.Token);
                        var bot = new UnifyBot.Bot(connectConfig);
                        ReciverMsg.Instance.BotStart(true, bot);
                        return AjaxResult(ReciverMsg.Instance.Bot!.Conn.CanConnetBot);
                    }
                    else
                    {
                        if (ReciverMsg.Instance.Bot != null)
                        {
                            ReciverMsg.Instance.Bot.Dispose();
                            ReciverMsg.Instance.Bot = null;
                        }
                    }
                }
                return Success();
            }
            catch (Exception e)
            {
                return Failed(e.Message);
            }
        }

        /// <summary>
        /// 搜索群
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet("searchgroup")]
        public ApiResult SearchGroup(string host, int wsPort, int httpPort, string token = "", string keywords = "")
        {
            try
            {
                if (TempBot == null)
                {
                    var connectConfig = new Connect(host, wsPort, httpPort, token);
                    var bot = new UnifyBot.Bot(connectConfig);
                    TempBot = bot;
                }
                if (TempBot == null) return DataResult(new List<GroupInfo>());
                var groupList = TempBot.Groups.Where(x => x.GroupName.Contains(keywords) || x.GroupQQ.ToString().Contains(keywords)).ToList();
                return DataResult(groupList);
            }
            catch (Exception)
            {
                TempBot?.Dispose();
                TempBot = null;
                return Failed("机器人服务可能不存在，请核对bot配置项，或者重新尝试搜索！");
            }
        }

        /// <summary>
        /// 搜索群中的人作为管理员
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet("searchgroupmember")]
        public ApiResult SearchGroupMember(string groups, string host, int wsPort, int httpPort, string token = "", string keywords = "")
        {
            try
            {
                if (TempBot == null)
                {
                    var connectConfig = new Connect(host, wsPort, httpPort, token);
                    var bot = new UnifyBot.Bot(connectConfig);
                    TempBot = bot;
                }
                if (TempBot == null) return DataResult(new List<GroupMemberInfo>());
                var groupids = groups.ToListInt().Select(x => (long)x);
                var groupList = TempBot.Groups.Where(x => groupids.Contains(x.GroupQQ)).ToList();
                var members = new List<GroupMemberInfo>();
                foreach (var group in groupList)
                {
                    members.AddRange(group.Members.Where(x => x.Nickname.Contains(keywords) || x.QQ.ToString().Contains(keywords)));
                }
                return DataResult(members);
            }
            catch (Exception)
            {
                TempBot?.Dispose();
                TempBot = null;
                return Failed("机器人服务可能不存在，请核对bot配置项，或者重新尝试搜索！");
            }
        }

        /// <summary>
        /// 搜索好友为超级管理员
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet("searchfriend")]
        public ApiResult SearchFriend(string host, int wsPort, int httpPort, string token = "", string keywords = "")
        {
            try
            {
                if (TempBot == null)
                {
                    var connectConfig = new Connect(host, wsPort, httpPort, token);
                    var bot = new UnifyBot.Bot(connectConfig);
                    TempBot = bot;
                }
                if (TempBot == null) return DataResult(new List<FriendInfo>());
                var friends = TempBot.Friends.Where(x => x.Nickname.Contains(keywords) || x.QQ.ToString().Contains(keywords)).ToList();
                return DataResult(friends);
            }
            catch (Exception)
            {
                TempBot?.Dispose();
                TempBot = null;
                return Failed("机器人服务可能不存在，请核对bot配置项，或者重新尝试搜索！");
            }
        }

        /// <summary>
        /// 获取qq信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("botinfo")]
        public ApiResult BotInfo()
        {
            dynamic obj = new System.Dynamic.ExpandoObject();
            return DataResult(obj);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("getconfig")]
        public async Task<ApiResult> GetConfig()
        {
            var config = await _sysConfig.GetConfig();
            return DataResult(config);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("saveconfig")]
        public async Task<ApiResult> SaveConfig(Config config)
        {
            var b = await _sysConfig.SaveConfig(config);
            if (b)
            {
                var enable = JObject.FromObject(config.EnableModule);
                foreach (var item in enable)
                {
                    if (item.Value.ObjToBool() == false)
                    {
                        var job = JobManager.GetSchedule(item.Key);
                        if (job != null)
                        {
                            job.Disable();
                            if (item.Key == "WB")
                            {
                                job = JobManager.GetSchedule("WBChiGua");
                                job.Disable();
                            }
                        }
                    }
                    else
                    {
                        var job = JobManager.GetSchedule(item.Key);
                        if (job != null)
                        {
                            job.Enable();
                            if (item.Key == "WB")
                            {
                                job = JobManager.GetSchedule("WBChiGua");
                                job.Enable();
                            }
                        }
                    }
                }
            }
            if (b) TempBot?.Dispose();
            return AjaxResult(b);
        }

        /// <summary>
        /// 接受口袋推送的消息
        /// </summary>
        /// <param name="msgBody"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost("postmsg")]
        public async Task<ApiResult> PostMsg([FromBody] Message msgBody, int type = 0)
        {
            if (type == 0)
                await Pocket.Instance.PocketMessageReceiver(msgBody.Content);
            if (type == 1)
                await Pocket.Instance.LiveMsgReceiver(msgBody.Content);
            return Success();
        }

        /// <summary>
        /// 获取缓存图片
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("getcache")]
        public async Task<ApiResult> GetCache([FromQuery] PageModel page)
        {
            var res = await _sysCache.GetPageListAsync(page);
            return ListResult(res.List, res.Count);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("saveimg/{id}")]
        public async Task<ApiResult> SaveImg(int id)
        {
            var model = await _sysCache.GetModelAsync(id);
            var b = await new FileHelper().Save(model.Content);
            if (b) await _sysCache.DeleteAsync(id);
            return AjaxResult(b);
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("delimg/{id}")]
        public async Task<ApiResult> DelImg(int id)
        {
            var b = await _sysCache.DeleteAsync(id);
            return AjaxResult(b);
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("delimgs")]
        public async Task<ApiResult> DelImg([FromBody] List<int> ids)
        {
            var b = await _sysCache.DeleteAsync(ids);
            return AjaxResult(b);
        }

        /// <summary>
        /// 获取qq插件功能
        /// </summary>
        /// <returns></returns>
        [HttpGet("getfun")]
        public ApiResult GetFun()
        {
            PluginHelper.LoadPlugins();
            var plugins = PluginHelper.Plugins;
            plugins.ForEach(x =>
            {
                if (!System.IO.File.Exists(x.PluginInfo!.ConfPath))
                    x.PluginInfo.ConfPath = "";
                if (!System.IO.File.Exists(x.PluginInfo.LogPath))
                    x.PluginInfo.LogPath = "";
            });
            return DataResult(plugins);
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <returns></returns>
        [HttpGet("startplugin")]
        public ApiResult StartPlugin(string name, string version)
        {
            var (b, msg) = PluginHelper.StartPlugin(name, version);
            return AjaxResult(b, msg);
        }

        /// <summary>
        /// 禁用插件
        /// </summary>
        /// <returns></returns>
        [HttpGet("stopplugin")]
        public ApiResult StopPlugin(string name)
        {
            var (b, msg) = PluginHelper.StopPlugin(name);
            return AjaxResult(b, msg);
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <returns></returns>
        [HttpGet("delplugin")]
        public ApiResult DelPlugin(string name)
        {
            try
            {
                var b = PluginHelper.DelPlugin(name);
                return AjaxResult(b);
            }
            catch (Exception e)
            {
                return Failed(e.Message);
            }
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        [HttpGet("openpluginconf")]
        public ApiResult OpenPluginConf(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return Failed("此插件没有配置文件");
            if (!System.IO.File.Exists(path)) return Failed("文件不存在");
            Process.Start("notepad.exe", path);
            return Success("打开成功！");
        }

        /// <summary>
        /// 启动阿里云盘
        /// </summary>
        /// <returns></returns>
        [HttpGet("startaliyunpan")]
        public ApiResult StartAliYunPan()
        {
            if (Config.EnableModule.BD && Config.BD.SaveAliyunDisk)
            {
                if (AliProcess != null)
                {
                    AliProcess.Kill();
                    AliProcess = null;
                }
                Task.Run(() =>
                {
                    if (Process.GetProcessesByName("wwwroot/script/alipan-win").Length == 0)
                    {
                        ProcessStartInfo startInfo = new()
                        {
                            FileName = "wwwroot/script/alipan-win.exe",
                            WorkingDirectory = Directory.GetCurrentDirectory(),
                            CreateNoWindow = true
                        };
                        AliProcess = Process.Start(startInfo)!;
                    }
                });
            }
            return Success();
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<ApiResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Failed("文件为空");
            }
            var dir = "wwwroot/images/standard";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string fileExtension = Path.GetExtension(file.FileName);
            var name = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExtension;
            var full = Path.Combine(dir, name);
            if (!System.IO.File.Exists(full))
            {
                using var stream = new FileStream(full, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            string url = "/images/standard/" + name;
            if (_env.IsDevelopment())
            {
                var ip = GetLocalIpAddress();
                var domain = Helper.ConfigHelper.GetConfiguration("urls").Replace("*", ip);
                url = Path.Combine(domain, url);
            }
            object obj = new
            {
                name,
                url
            };
            return DataResult(obj);
        }
        static string GetLocalIpAddress()
        {
            string ipAddress = string.Empty;
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }

            return ipAddress;
        }

        /// <summary>
        /// 上传qq插件
        /// </summary>
        /// <returns></returns>
        [HttpPost("uploaddll")]
        public async Task<ApiResult> Uploaddll(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Failed("文件为空");
            }
            var dir = "Plugins";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var full = Path.Combine(dir, file.FileName);
            if (!System.IO.File.Exists(full))
            {
                using var stream = new FileStream(full, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            return Success();
        }

        /// <summary>
        /// 搜索48小偶像
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpGet("searchidol")]
        public async Task<ApiResult> SearchIdol(string name, string group = "")
        {
            string url = @"https://fastly.jsdelivr.net/gh/duan602728596/qqtools@main/packages/NIMTest/node/roomId.json";
            var chain = new List<SysIdol>();
            SysIdol? xox = new();
            if (group != "")
            {
                var groupList = group.Split(",").ToList();
                if (groupList.Count >= 2)
                    chain = await _sysIdol.GetListAsync(t => t.GroupName == groupList[0] && t.Team == groupList[1] && t.Name.Contains(name));
                else
                    chain = await _sysIdol.GetListAsync(t => t.GroupName == groupList[0] && t.Name.Contains(name));
            }
            else
            {
                chain = await _sysIdol.GetListAsync(t => t.Name.Contains(name));
            }
            return Success(chain, url);
        }

        /// <summary>
        /// 通过id保存微博
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        [HttpGet("savewbbyid")]
        public ApiResult SaveWbById(string blogId)
        {
            Task.Run(async () => { await new Weibo().SaveByUrl(blogId); });
            return Success("正在进行保存中！");
        }

        /// <summary>
        /// 口袋登录发送验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        [HttpGet("sendsmscode")]
        public async Task<ApiResult> SendSmsCode(string mobile, string area = "86")
        {
            var res = await Pocket.SmsCode(mobile, area);
            if (res.Fetch<bool>("success"))
                return DataResult(res.Fetch("content"));
            else
                return Failed(res.Fetch("message"));
        }

        /// <summary>
        /// 口袋登录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("pocketlogin")]
        public async Task<ApiResult> PocketLogin(string mobile, string code)
        {
            var res = await Pocket.Login(mobile, code);
            if (res.Fetch<bool>("success"))
                return DataResult(res.Fetch("content"));
            else
                return Failed(res.Fetch("message"));
        }

        /// <summary>
        /// 口袋个人信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("kduserinfo")]
        public async Task<ApiResult> KDUserInfo()
        {
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            string requestBody = await reader.ReadToEndAsync();
            var token = requestBody.Fetch("token");
            var res = await Pocket.UserInfo(token);
            if (res.Fetch<bool>("success"))
                return DataResult(res.Fetch("content"));
            else
                return Failed(res.Fetch("message"));
        }

        /// <summary>
        /// 获取错误日志最新10条
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("getlogs")]
        public async Task<ApiResult> GetLogs()
        {
            var logs = await _sysLog.GetListAsync(10);
            return DataResult(logs);
        }
    }
}
