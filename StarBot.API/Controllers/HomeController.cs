using StarBot.Entity;
using StarBot.Extension;
using StarBot.IService;
using StarBot.Model;
using StarBot.Timer;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using ShamrockCore;
using System.Diagnostics;

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
        /// ����
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
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        [HttpGet("getconfig")]
        public async Task<ApiResult> GetConfig()
        {
            var config = await _sysConfig.GetConfig();
            return DataResult(config);
        }

        /// <summary>
        /// ��������
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
        /// ��ȡ����ͼƬ����
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
        /// ���滺��ͼƬ
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
        /// ɾ������ͼƬ
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
        /// ��ȡqq����
        /// </summary>
        /// <returns></returns>
        [HttpGet("getfun")]
        public ApiResult GetFun()
        {
            var funs = new PluginHelper();
            return DataResult(funs.Plugins);
        }

        /// <summary>
        /// ������������
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
        /// �ϴ��ļ�
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
        /// ����Сż��
        /// </summary>
        /// <param name="group">����,�ֶ�</param>
        /// <param name="name">����</param>
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
            if (xox == null) return Success(url, "δ��ѯ����Сż��");
            return DataResult(xox);
        }

        /// <summary>
        /// ����΢����̬ͼƬ
        /// </summary>
        /// <param name="blogId">΢��id</param>
        /// <returns></returns>
        [HttpGet("savewbbyid")]
        public ApiResult SaveWbById(string blogId)
        {
            Task.Run(async () => { await new Weibo().SaveByUrl(blogId); });
            return Success("������΢����ͼƬʶ�𱣴�����У�");
        }

        /// <summary>
        /// �ڴ���¼��֤��
        /// </summary>
        /// <param name="mobile">�ֻ���</param>
        /// <param name="area">����</param>
        /// <returns></returns>
        [HttpGet("sendsmscode")]
        public async Task<ApiResult> SendSmsCode(string mobile, string area = "86")
        {
            var res = await Pocket.SmsCode(mobile, area);
            return DataResult(res);
        }

        /// <summary>
        /// �ڴ���¼
        /// </summary>
        /// <param name="mobile">�ֻ���</param>
        /// <param name="code">��֤��</param>
        /// <returns></returns>
        [HttpGet("pocketlogin")]
        public async Task<ApiResult> PocketLogin(string mobile, string code)
        {
            var res = await Pocket.Login(mobile, code);
            return DataResult(res);
        }

        /// <summary>
        /// ��ȡ�ڴ���¼��Ϣ
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
