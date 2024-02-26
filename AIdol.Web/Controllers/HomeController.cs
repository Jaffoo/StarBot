using AIdol.Extension;
using AIdol.IService;
using AIdol.Model;
using Microsoft.AspNetCore.Mvc;
using ICacheService = AIdol.Extension.ICacheService;

namespace AIdol.Controllers
{
    public class HomeController(ISysConfig sysConfig) : BaseController
    {
        ISysConfig _sysConfig = sysConfig;

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
            return AjaxResult(await _sysConfig.SaveConfig(config));
        }
    }
}
