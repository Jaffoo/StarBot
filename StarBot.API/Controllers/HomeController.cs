using StarBot.Entity;
using StarBot.Extension;
using StarBot.IService;
using StarBot.Model;
using StarBot.Timer;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using ShamrockCore;
using System.Diagnostics;
using TBC.CommonLib;
using System.Xml.Linq;
using ElectronNET.API;
using Newtonsoft.Json.Linq;
using SqlSugar.Extensions;

namespace StarBot.Controllers
{
    public class HomeController(ISysConfig sysConfig, ISysCache sysCache, ISysIdol sysIdol) : BaseController
    {
        ISysConfig _sysConfig = sysConfig;
        ISysCache _sysCache = sysCache;
        ISysIdol _sysIdol = sysIdol;

        public Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }

        /// <summary>
        /// 启动qq机器人
        /// </summary>
        /// <returns></returns>
        [HttpGet("startbot")]
        public ApiResult StartBot()
        {
            var connectConfig = new ConnectConfig(Config.Shamrock.Host, Config.Shamrock.WebsocktPort, Config.Shamrock.HttpPort, Config.Shamrock.Token);
            var bot = new Bot(connectConfig);
            ReciverMsg.Instance.BotStart(bot);
            JobManager.Initialize(new FluentSchedulerFactory());
            return Success();
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
                        job.Disable();
                        if (item.Key == "WB")
                        {
                            job = JobManager.GetSchedule("WBChiGua");
                            job.Disable();
                        }
                    }
                    else
                    {
                        var job = JobManager.GetSchedule(item.Key);
                        job.Enable();
                        if (item.Key == "WB")
                        {
                            job = JobManager.GetSchedule("WBChiGua");
                            job.Enable();
                        }
                    }
                }
            }
            return AjaxResult(b);
        }

        /// <summary>
        /// 接受口袋推送的消息
        /// </summary>
        /// <param name="msgBody"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost("postmsg")]
        public async Task<ApiResult> PostMsg([FromBody] string msgBody, int type = 0)
        {
            if (type == 0)
                await Pocket.Instance.PocketMessageReceiver(msgBody);
            if (type == 1)
                await Pocket.Instance.LiveMsgReceiver(msgBody);
            return Success();
        }

        /// <summary>
        /// 获取缓存图片
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("getcache")]
        public async Task<ApiResult> GetCache()
        {
            var res = await _sysCache.GetListAsync();
            return DataResult(res);
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
        /// 获取qq插件功能
        /// </summary>
        /// <returns></returns>
        [HttpGet("getfun")]
        public ApiResult GetFun()
        {
            PluginHelper.LoadPlugins();
            var plugins = PluginHelper.Plugins;
            return DataResult(plugins);
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <returns></returns>
        [HttpGet("startplugin")]
        public ApiResult StartPlugin(string name)
        {
            var (b, msg) = PluginHelper.StartPlugin(name);
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
            var b = PluginHelper.DelPlugin(name);
            return AjaxResult(b);
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
                Task.Run(() =>
                {
                    var path = "wwwroot/script/AliDiskApi.exe";
                    using Process p = Process.Start(path)!;
                });
            }
            return Success();
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public ApiResult Upload(string path)
        {

            FileInfo fileInfo = new(path);
            var dir = "wwwroot/images/standard";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var fileName = name + fileInfo.Extension;
            var full = Path.Combine(dir, fileName);
            if (!System.IO.File.Exists(full))
                fileInfo.CopyTo(full);
            else
            {
                System.IO.File.Delete(full);
                fileInfo.CopyTo(full);
            }
            object obj = new
            {
                name,
                url = "/images/standard" + fileName
            };
            return DataResult(obj);
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
                    chain = await _sysIdol.GetListAsync(t => t.GroupName == groupList[0] && t.Team == groupList[1]);
                else
                    chain = await _sysIdol.GetListAsync(t => t.GroupName == groupList[0]);
            }
            else
            {
                chain = await _sysIdol.GetListAsync(t => t.Name.Contains(name));
            }
            xox = chain?.FirstOrDefault(t => t.Name.Contains(name));
            if (xox == null) return Failed(url);
            return DataResult(xox);
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
            return DataResult(res.ToJObject());
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
            return DataResult(res.ToJObject());
        }

        /// <summary>
        /// 口袋个人信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("kduserinfo")]
        public async Task<ApiResult> KDUserInfo(string token)
        {
            var res = await Pocket.UserInfo(token);
            return DataResult(res.ToJObject());
        }

        /// <summary>
        /// 打开新窗口
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("openwindow")]
        public async Task<ApiResult> OpenWindow(string url)
        {
            // 使用 Electron.NET 的 API 创建一个新窗口
            await Electron.WindowManager.CreateWindowAsync(url);
            return Success();
        }
    }
}
