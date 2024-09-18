using StarBot.Helper;
using SqlSugar;
using System.Reflection;
using Newtonsoft.Json;

namespace StarBot.Repository
{
    /// <summary>
    /// 数据库连接工厂
    /// </summary>
    public class DBFactory
    {
        public static void InitTable()
        {
            #region 数据库创建/更新
            if (!File.Exists("wwwroot/data/main.db"))
            {
                InitInstance().DbMaintenance.CreateDatabase();
            }

            Assembly assembly = Assembly.Load("StarBot.Entity");
            var classes = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType).ToList();

            foreach (var item in classes)
            {
                try
                {
                    InitInstance().CodeFirst.InitTables(item);
                }
                catch
                {
                    continue;
                }
            }

            if (File.Exists("wwwroot/data/main.sql"))
            {
                var sqlList = File.ReadAllLines("wwwroot/data/main.sql");
                foreach (var sql in sqlList)
                {
                    try
                    {
                        InitInstance().Ado.ExecuteCommand(sql);
                    }
                    catch (Exception e)
                    {
                        UtilHelper.WriteLog(e.Message,prefix:"updateDB");
                    }
                }
            }

            if (File.Exists("wwwroot/data/update.sql"))
            {
                var sqlStr = File.ReadAllText("wwwroot/data/update.sql");
                InitInstance().Ado.ExecuteCommand(sqlStr);
            }
            #endregion
        }
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
                SlaveConnectionConfigs = [new() { HitRate = 10, ConnectionString = slave }],
                ConfigureExternalServices = new()
                {
                    //注意:  这儿AOP设置不能少
                    EntityService = (c, p) =>
                    {
                        if (p.IsPrimarykey == false && new NullabilityInfoContext()
                         .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }
                    }
                }
            };
            SqlSugarClient sqlSugarClient = new(config);

            sqlSugarClient.Aop.OnLogExecuted = (sql, param) =>
            {
                var nativeSql = UtilMethods.GetNativeSql(sql, param);
            };
            sqlSugarClient.Aop.OnError = (ex) =>
            {
                UtilHelper.WriteLog(UtilMethods.GetNativeSql(ex.Sql, ex.Parametres as SugarParameter[]));
            };
            return sqlSugarClient;
        }
    }
}