using SqlSugar;
using SqlSugar.Extensions;

namespace AIdol.Model
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public bool this[string key, string subKey]
        {
            get
            {
                var prop = typeof(Config).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var config = new Config();
                var propValue = prop.GetValue(config); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var config = new Config(); // 索引器中已经实例化过的Config类的实例

                var prop = typeof(Config).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                // 获取key属性的值
                var keyPropertyValue = prop.GetValue(config);

                if (keyPropertyValue != null)
                {
                    // 假设key属性是一个类，且你想要设置这个类中的某个属性的值
                    var keyProperty = keyPropertyValue.GetType().GetProperty(subKey);
                    if (keyProperty != null)
                    {
                        // 设置属性值为给定的value
                        keyProperty.SetValue(keyPropertyValue, value);
                    }
                }
            }
        }

        /// <summary>
        /// 启用项
        /// </summary>
        public EnableModule EnableModule { get; set; } = new();

        /// <summary>
        /// Shamrock配置
        /// </summary>
        public Shamrock Shamrock { get; set; } = new();

        /// <summary>
        /// 百度
        /// </summary>
        public BD BD { get; set; } = new();

        /// <summary>
        /// qq
        /// </summary>
        public QQ QQ { get; set; } = new();

        /// <summary>
        /// 小红书
        /// </summary>
        public XHS XHS { get; set; } = new();

        /// <summary>
        /// 抖音
        /// </summary>
        public DY DY { get; set; } = new();

        /// <summary>
        /// 口袋
        /// </summary>
        public KD KD { get; set; } = new();

        /// <summary>
        /// B站
        /// </summary>
        public BZ BZ { get; set; } = new();

        /// <summary>
        /// 微博
        /// </summary>
        public WB WB { get; set; } = new();
    }

    /// <summary>
    /// Shamrock
    /// </summary>
    public class Shamrock
    {
        /// <summary>
        /// 使用
        /// </summary>
        public bool Use { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Websockt端口
        /// </summary>
        public int WebsocktPort { get; set; }

        /// <summary>
        /// http端口
        /// </summary>
        public int HttpPort { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; } = "";
    }

    /// <summary>
    /// 启用项
    /// </summary>
    public class EnableModule
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// qq
        /// </summary>
        public bool QQ { get; set; }

        /// <summary>
        /// 微博
        /// </summary>
        public bool WB { get; set; }

        /// <summary>
        /// 哔哩哔哩
        /// </summary>
        public bool BZ { get; set; }

        /// <summary>
        /// 口袋48
        /// </summary>
        public bool KD { get; set; }

        /// <summary>
        /// 小红书
        /// </summary>
        public bool XHS { get; set; }

        /// <summary>
        /// 抖音
        /// </summary>
        public bool DY { get; set; }

        /// <summary>
        /// 百度人脸
        /// </summary>
        public bool BD { get; set; }
    }

    /// <summary>
    /// 百度
    /// </summary>
    public class BD
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// appkey
        /// </summary>
        public string AppKey { get; set; } = string.Empty;

        /// <summary>
        /// 密钥
        /// </summary>
        public string AppSeret { get; set; } = string.Empty;

        /// <summary>
        /// 相似度
        /// </summary>
        public int Similarity { get; set; }

        /// <summary>
        /// 保存到阿里云盘
        /// </summary>
        public bool SaveAliyunDisk { get; set; }

        /// <summary>
        /// 审核
        /// </summary>
        public int Audit { get; set; }

        /// <summary>
        /// 相册名称
        /// </summary>
        public string AlbumName { get; set; } = string.Empty;

        /// <summary>
        /// 人脸对比
        /// </summary>
        public bool FaceVerify { get; set; }

        /// <summary>
        /// 用于对比的基础图片
        /// </summary>
        public List<string> ImageList { get; set; } = [];
    }

    /// <summary>
    /// 口袋
    /// </summary>
    public class KD
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// 小偶像名称
        /// </summary>
        public string IdolName { get; set; } = string.Empty;

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 服务器id
        /// </summary>
        public string ServerId { get; set; } = string.Empty;

        /// <summary>
        /// qq群
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 是否转发到qq群
        /// </summary>
        public bool ForwardGroup { get; set; }

        /// <summary>
        /// 是否转发到qq
        /// </summary>
        public bool ForwardQQ { get; set; }

        /// <summary>
        /// qq号
        /// </summary>
        public string QQ { get; set; } = string.Empty;

        /// <summary>
        /// appkey
        /// </summary>
        public string AppKey { get; set; } = string.Empty;

        /// <summary>
        /// 图片域名
        /// </summary>
        public string ImgDomain { get; set; } = string.Empty;

        /// <summary>
        /// 视频域名
        /// </summary>
        public string VideoDomain { get; set; } = string.Empty;

        /// <summary>
        /// 直播房间id
        /// </summary>
        public string LiveRoomId { get; set; } = string.Empty;

        /// <summary>
        /// 消息类型
        /// </summary>
        public List<MsgTypeModel> MsgTypeAll { get; set; } = [];

        /// <summary>
        /// 接收/转发消息类型
        /// </summary>
        public List<string> MsgType { get; set; } = [];

        /// <summary>
        /// 0-不保存，1-小偶像消息，2-全部消息
        /// </summary>
        public int SaveMsg { get; set; }
    }

    /// <summary>
    /// 微博
    /// </summary>
    public class WB
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// 用户id
        /// 转发动态，保存图片
        /// </summary>
        public string UserAll { get; set; } = string.Empty;

        /// <summary>
        /// 用户id
        /// 仅用于保存图片
        /// </summary>
        public string UserPart { get; set; } = string.Empty;

        /// <summary>
        /// 监听间隔时间
        /// </summary>
        public int TimeSpan { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 转发qq群
        /// </summary>
        public bool ForwardGroup { get; set; }

        /// <summary>
        /// 转发qq
        /// </summary>
        public bool ForwardQQ { get; set; }

        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; } = string.Empty;

        /// <summary>
        /// 吃瓜id
        /// </summary>
        public string ChiGuaUser { get; set; } = string.Empty;

        /// <summary>
        /// 吃瓜微博关键词过滤
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 吃瓜转发qq
        /// </summary>
        public string ChiGuaQQ { get; set; } = string.Empty;

        /// <summary>
        /// 吃瓜转发qq
        /// </summary>
        public bool ChiGuaForwardQQ { get; set; }

        /// <summary>
        /// 吃瓜转发群
        /// </summary>
        public string ChiGuaGroup { get; set; } = string.Empty;

        /// <summary>
        /// 吃瓜转发群
        /// </summary>
        public bool ChiGuaForwardGroup { get; set; }
    }

    /// <summary>
    /// 抖音
    /// </summary>
    public class DY
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// 用户id
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// 监听间隔时间
        /// </summary>
        public int TimeSpan { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 转发qq群
        /// </summary>
        public bool ForwardGroup { get; set; }

        /// <summary>
        /// 转发qq
        /// </summary>
        public bool ForwardQQ { get; set; }

        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; } = string.Empty;
    }

    /// <summary>
    /// 小红书
    /// </summary>
    public class XHS
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// 用户id
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// 监听间隔时间
        /// </summary>
        public int TimeSpan { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 转发qq群
        /// </summary>
        public bool ForwardGroup { get; set; }

        /// <summary>
        /// 转发qq
        /// </summary>
        public bool ForwardQQ { get; set; }

        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; } = string.Empty;
    }

    /// <summary>
    /// bilibili
    /// </summary>
    public class BZ
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// 用户id
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// 监听间隔时间
        /// </summary>
        public int TimeSpan { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 转发qq群
        /// </summary>
        public bool ForwardGroup { get; set; }

        /// <summary>
        /// 转发qq
        /// </summary>
        public bool ForwardQQ { get; set; }

        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; } = string.Empty;
    }

    /// <summary>
    /// qq
    /// </summary>
    public class QQ
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[string key]
        {
            get
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return false;
                }
                var enableModule = new EnableModule();
                var propValue = prop.GetValue(enableModule); // 获取属性值
                if (propValue != null)
                {
                    return propValue.ObjToBool();
                }
                return false;
            }
            set
            {
                var prop = typeof(EnableModule).GetProperty(key);
                if (prop == null)
                {
                    return; // 如果属性不存在，直接返回
                }

                var enableModule = new EnableModule(); // 假设EnableModule是包含要设置属性的类的实例
                prop.SetValue(enableModule, value); // 设置属性值
            }
        }

        /// <summary>
        /// 管理员使用的扩展功能
        /// </summary>
        public List<string> FuncAdmin { get; set; } = [];

        /// <summary>
        /// 监听群/转发群
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 是否保存群消息
        /// </summary>
        public bool Save { get; set; }

        /// <summary>
        /// 超管
        /// </summary>
        public string Admin { get; set; } = string.Empty;

        /// <summary>
        /// 开启消息通知
        /// </summary>
        public bool Notice { get; set; } = true;

        /// <summary>
        /// 管理员
        /// </summary>
        public string Permission { get; set; } = string.Empty;

        /// <summary>
        /// 开启程序错误通知
        /// </summary>
        public bool Debug { get; set; } = false;
    }

    /// <summary>
    /// 口袋消息类型
    /// </summary>
    public class MsgTypeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
