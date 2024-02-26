namespace AIdol.Model
{
    /// <summary>
    /// 枚举
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// 调试(中)
            /// </summary>
            DEBUG,

            /// <summary>
            /// 信息(低)
            /// </summary>
            INFO,

            /// <summary>
            /// 警告(中)
            /// </summary>
            WARNING,

            /// <summary>
            /// 错误（高）
            /// </summary>
            ERROR,

            /// <summary>
            /// 严重（高）
            /// </summary>
            CRITICAL
        }

        /// <summary>
        /// 文件上传类型
        /// </summary>
        public enum UploadType
        {
            /// <summary>
            /// 默认目录
            /// </summary>
            Upload,

            /// <summary>
            /// 系统目录
            /// </summary>
            System,
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// 图片
            /// </summary>
            Image,

            /// <summary>
            /// 办公文件
            /// </summary>
            Office,

            /// <summary>
            /// 视频
            /// </summary>
            Video,

            /// <summary>
            /// 其他
            /// </summary>
            Other
        }
    }
}
