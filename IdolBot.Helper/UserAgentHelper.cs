namespace IdolBot.Helper
{
    public class UserAgentHelp
    {
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="UserAgentText"></param>
        /// <returns></returns>
        public static UserAgentDeviceInfoModel GetDeviceInfo(string UserAgentText)
        {
            UserAgentDeviceInfoModel userAgentDeviceInfoModel = new()
            {
                System = ChackSystemInfo(UserAgentText),
                App = ChackAppInfo(UserAgentText),
                IsMobile = UserAgentText.Contains("Mobile")
            };
            userAgentDeviceInfoModel.Remark = $"系统：{userAgentDeviceInfoModel.System}；应用名称：{userAgentDeviceInfoModel.App}；是否移动端：{userAgentDeviceInfoModel.IsMobile}；";

            return userAgentDeviceInfoModel;
        }
        /// <summary>
        /// 检查系统信息
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static DeviceSystemEnum ChackSystemInfo(string str)
        {
            if (str.Contains("Android"))
            {
                return DeviceSystemEnum.Android;
            }
            else if (str.Contains("iPhone"))
            {
                return DeviceSystemEnum.iPhone;
            }
            else if (str.Contains("Windows"))
            {
                return DeviceSystemEnum.Windows;
            }
            else
            {
                return DeviceSystemEnum.Other;//未知
            }
        }
        /// <summary>
        /// 检查应用信息
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static DeviceAppEnum ChackAppInfo(string str)
        {
            if (str.Contains("MicroMessenger"))
            {
                return DeviceAppEnum.WeChat;
            }
            else if (str.Contains("AlipayClient"))
            {
                return DeviceAppEnum.Alipay;
            }
            else if (str.Contains("QQ"))
            {
                return DeviceAppEnum.QQ;
            }
            else
            {
                return DeviceAppEnum.Other;//未知
            }
        }
    }
    /// <summary>
    /// 用户请求的设备信息
    /// </summary>
    public class UserAgentDeviceInfoModel
    {
        /// <summary>
        /// 系统信息
        /// </summary>
        public DeviceSystemEnum System { set; get; }
        /// <summary>
        /// 应用信息
        /// </summary>
        public DeviceAppEnum App { set; get; }
        /// <summary>
        /// 是否移动端
        /// </summary>
        public bool IsMobile { set; get; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string? Remark { set; get; }
    }

    /// <summary>
    /// 系统信息枚举
    /// </summary>
    public enum DeviceSystemEnum
    {
        Android,
        iPhone,
        Windows,
        /// <summary>
        /// 未知
        /// </summary>
        Other,
    }

    /// <summary>
    /// 应用名称信息枚举
    /// </summary>
    public enum DeviceAppEnum
    {
        /// <summary>
        /// 微信-(判断字段MicroMessenger)
        /// </summary>
        WeChat,
        /// <summary>
        /// 支付宝-(判断字段AlipayClient)
        /// </summary>
        Alipay,
        /// <summary>
        /// qq-(判断字段QQ)
        /// </summary>
        QQ,
        /// <summary>
        /// 未知
        /// </summary>
        Other,
    }
}