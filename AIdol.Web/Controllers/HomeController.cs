using AIdol.Extension;
using AIdol.IService;
using AIdol.Model;
using Helper;
using Microsoft.AspNetCore.Mvc;

namespace AIdol.Controllers
{
    public class HomeController(ISysConfig sysConfig, ISysCache sysCache) : BaseController
    {
        ISysConfig _sysConfig = sysConfig;
        ISysCache _sysCache = sysCache;

        /// <summary>
        /// ªÒ»°≈‰÷√
        /// </summary>
        /// <returns></returns>
        [HttpGet("getconfig")]
        public async Task<ApiResult> GetConfig()
        {
            var config = await _sysConfig.GetConfig();
            return DataResult(config);
        }

        /// <summary>
        /// ±£¥Ê≈‰÷√
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
        /// ªÒ»°ª∫¥ÊÕº∆¨ ˝æ›
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<ApiResult> GetCache([FromQuery] PageModel page)
        {
            var res = await _sysCache.GetPageListAsync(page);
            return ListResult(res.List, res.Count);
        }

        /// <summary>
        /// ±£¥Êª∫¥ÊÕº∆¨
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("saveimg")]
        public async Task<ApiResult> SaveImg(int id)
        {
            var model = await _sysCache.GetModelAsync(id);
            var b = await new FileHelper().Save(model.Content);
            return Success(b);
        }

        /// <summary>
        /// …æ≥˝ª∫¥ÊÕº∆¨
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("delimg")]
        public async Task<ApiResult> DelImg(int id)
        {
            var b = await _sysCache.DeleteAsync(id);
            return AjaxResult(b);
        }
    }
}
