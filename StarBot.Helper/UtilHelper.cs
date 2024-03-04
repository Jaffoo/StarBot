namespace StarBot.Helper
{
    /// <summary>
    /// 本地工具类
    /// </summary>
    public static class UtilHelper
    {
        #region 本地日志
        static readonly object _lockObj = new();
        /// <summary>
        /// 本地日志
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="folder">文件夹</param>
        /// <param name="prefix">文件前缀</param>
        public static void WriteLog(string content, string folder = "logs", string prefix = "")
        {
            lock (_lockObj)
            {
                folder = AppDomain.CurrentDomain.BaseDirectory + folder;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string fileName = $"{prefix}_{DateTime.Now:yyyyMMdd}.log";
                content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "：" + content;
                string path = Path.Combine(folder, fileName);
                //创建或打开日志文件，向日志文件末尾追加记录
                StreamWriter mySw = File.AppendText(path);
                //向日志文件写入内容
                mySw.WriteLine(content);
                //关闭日志文件
                mySw.Close();
            }
        }
        #endregion
    }
}
