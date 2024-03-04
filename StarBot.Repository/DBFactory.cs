using StarBot.Helper;
using SqlSugar;

namespace StarBot.Repository
{
    /// <summary>
    /// 数据库连接工厂
    /// </summary>
    public class DBFactory
    {
        /// <summary>
        /// 获取数据库实例
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient InitInstance()
        {
            var master = ConfigHelper.GetConfiguration("ConnectionStrings:ConnectionString_Master");
            var slave = ConfigHelper.GetConfiguration("ConnectionStrings:ConnectionString_Slave");
            var dbType = ConfigHelper.GetConfiguration("ConnectionStrings:DbType");
            ConnectionConfig config = new()
            {
                ConnectionString = master,
                DbType = (DbType)Enum.Parse(typeof(DbType), dbType),
                IsAutoCloseConnection = true,
                //从库/读库
                SlaveConnectionConfigs = [new() { HitRate = 10, ConnectionString = slave }]
            };
            SqlSugarClient sqlSugarClient = new(config);

            sqlSugarClient.Aop.OnLogExecuting = (sql, param) =>
            {
                Console.Write("参数：");
                foreach (var item in param)
                {
                    Console.Write(item.ParameterName + ":" + item.Value + " ");
                }
                Console.WriteLine("\n准备执行：" + sql);
            };
            sqlSugarClient.Aop.OnLogExecuted = (sql, param) =>
            {
                Console.Write("参数：");
                foreach (var item in param)
                {
                    Console.Write(item.ParameterName + ":" + item.Value + " ");
                }
                Console.WriteLine("\n执行完毕：" + sql);
            };
            return sqlSugarClient;
        }
    }
}