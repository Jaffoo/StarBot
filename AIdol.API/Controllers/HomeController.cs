using AIdol.Entity;
using AIdol.Extension;
using AIdol.IService;
using AIdol.Model;
using AIdol.Timer;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using ShamrockCore;
using System.Diagnostics;

namespace AIdol.Controllers
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
        /// 启动
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
            return AjaxResult(b);
        }

        [HttpPost("postmsg")]
        public async Task<ApiResult> PostMsg([FromBody] string msgBody, int type = 0)
        {
            if (type == 0)
                await Pocket.Instance.PocketMessageReceiver(msgBody);
            if (type == 1)
                await Pocket.Instance.LiveMsgReceiver(msgBody);
            return Success(true);
        }

        /// <summary>
        /// 获取缓存图片数据
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
        /// 保存缓存图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("saveimg/{id}")]
        public async Task<ApiResult> SaveImg(int id)
        {
            var model = await _sysCache.GetModelAsync(id);
            var b = await new FileHelper().Save(model.Content);
            return Success(b);
        }

        /// <summary>
        /// 删除缓存图片
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
        /// 获取qq功能
        /// </summary>
        /// <returns></returns>
        [HttpGet("getfun")]
        public ApiResult GetFun()
        {
            var funs = new PluginHelper();
            return DataResult(funs.Plugins);
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
                    var path = Directory.GetCurrentDirectory() + "/wwwroot/script/AliDiskApi.exe";
                    using Process p = Process.Start(path)!;
                });
            }
            return Success();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public ApiResult Upload(string path)
        {

            FileInfo fileInfo = new(path);
            var root = Directory.GetCurrentDirectory();
            root = Path.Combine(root, "wwwroot/images/standard");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            var name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var fileName = "/" + name + fileInfo.Extension;
            if (!System.IO.File.Exists(root + fileName))
                fileInfo.CopyTo(root + fileName);
            else
            {
                System.IO.File.Delete(root + fileName);
                fileInfo.CopyTo(root + fileName);
            }
            object obj = new
            {
                name,
                url = "/images/standard" + fileName
            };
            return DataResult(obj);
        }

        /// <summary>
        /// 搜索小偶像
        /// </summary>
        /// <param name="group">队伍,分队</param>
        /// <param name="name">名字</param>
        /// <returns></returns>
        [HttpGet("searchidol")]
        public async Task<ApiResult> SearchIdol(string group, string name)
        {
            var groupList = group.Split(",").ToList();
            string url = @"https://fastly.jsdelivr.net/gh/duan602728596/qqtools@main/packages/NIMTest/node/roomId.json";
            var chain = new List<SysIdol>();
            SysIdol? xox = new();
            if (groupList != null)
            {
                if (groupList.Count >= 2)
                    chain = await _sysIdol.GetListAsync(t => t.GroupName == groupList[0] && t.Team == groupList[1]);
                else
                    chain = await _sysIdol.GetListAsync(t => t.GroupName == groupList[0]);
            }
            xox = chain?.FirstOrDefault(t => t.Name == name);
            if (xox == null) return Success(url, "未查询到该小偶像");
            return DataResult(xox);
        }

        /// <summary>
        /// 保存微博动态图片
        /// </summary>
        /// <param name="blogId">微博id</param>
        /// <returns></returns>
        [HttpGet("savewbbyid")]
        public ApiResult SaveWbById(string blogId)
        {
            Task.Run(async () => { await new Weibo().SaveByUrl(blogId); });
            return Success("检索到微博！图片识别保存进行中！");
        }

        /// <summary>
        /// 口袋登录验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="area">区号</param>
        /// <returns></returns>
        [HttpGet("sendsmscode")]
        public async Task<ApiResult> SendSmsCode(string mobile, string area = "86")
        {
            var res = await Pocket.SmsCode(mobile, area);
            return DataResult(res);
        }

        /// <summary>
        /// 口袋登录
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpGet("pocketlogin")]
        public async Task<ApiResult> PocketLogin(string mobile, string code)
        {
            var res = await Pocket.Login(mobile, code);
            return DataResult(res);
        }

        /// <summary>
        /// 获取口袋登录信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("kduserinfo")]
        public async Task<ApiResult> KDUserInfo(string token)
        {
            var res = await Pocket.UserInfo(token);
            return DataResult(res);
        }
    }
}
