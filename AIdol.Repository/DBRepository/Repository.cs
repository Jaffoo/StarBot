using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using SqlSugar;
using PageModel = AIdol.Model.PageModel;

namespace AIdol.Repository
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        //定义数据访问上下文对象
        private readonly Lazy<SqlSugarClient> _db = new(() => DBFactory.InitInstance());

        /// <summary>
        /// 数据库对象实例
        /// </summary>
        protected SqlSugarClient Db => _db.Value;

        #region 同步

        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.SlaveQueryable<T>().AnyAsync(predicate);
        }

        #region 新增

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public async Task<int> AddAsync(T model)
        {
            var row = await Db.Insertable(model).ExecuteReturnIdentityAsync();
            return row;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        public async Task<int> AddAsync(T model, params string[] insterCol)
        {
            var row = await Db.Insertable(model).InsertColumns(insterCol).ExecuteReturnIdentityAsync();
            return row;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        public async Task<int> AddAsync(T model, Expression<Func<T, object>>? insterCol = null)
        {
            var row = await Db.Insertable(model).InsertColumns(insterCol).ExecuteReturnIdentityAsync();
            return row;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities, params string[] insterCol)
        {
            var row = await Db.Insertable(entities.ToList()).InsertColumns(insterCol).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns></returns>
        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            var row = await Db.Insertable(entities.ToList()).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities, Expression<Func<T, object>>? insterCol = null)
        {
            var row = await Db.Insertable(entities.ToList()).InsertColumns(insterCol).ExecuteCommandAsync();
            return row > 0;
        }
        #endregion
        #region 更新实体
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            int row = await Db.Updateable(entity).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="newEntity">老实体</param>
        /// <param name="oldEntity">新实体</param>
        public virtual async Task<bool> UpdateAsync(T newEntity, T oldEntity)
        {
            CompareAndAssign(oldEntity, newEntity);
            return await UpdateAsync(oldEntity);
        }

        /// <summary>
        /// 更新实体(危险操作，非必要不适用，使用请确保两个列表同索引的主键值一样)
        /// </summary>
        /// <param name="newEntities">老实体</param>
        /// <param name="oldEntities">新实体</param>
        public virtual async Task<bool> UpdateAsync(IEnumerable<T> newEntities, IEnumerable<T> oldEntities)
        {
            var newTemp = newEntities.ToList();
            var oldTemp = oldEntities.ToList();
            for (int i = 0; i < newTemp.Count; i++)
            {
                CompareAndAssign(oldTemp[i], newTemp[i]);
            }
            return await UpdateRangeAsync(oldTemp);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="updateCol">更新指定字段</param>
        public virtual async Task<bool> UpdateAsync(T entity, params string[] updateCol)
        {
            int row = await Db.Updateable(entity).UpdateColumns(updateCol).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="updateCol">更新指定字段</param>
        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, object>>? updateCol = null)
        {
            int row = await Db.Updateable(entity).UpdateColumns(updateCol).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
        {
            int row = await Db.Updateable(entities.ToList()).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="updateCol">更新指定字段</param>
        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> entities, params string[] updateCol)
        {
            int row = await Db.Updateable(entities.ToList()).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="updateCol">更新指定字段</param>
        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> entities, Expression<Func<T, object>>? updateCol = null)
        {
            int row = await Db.Updateable(entities.ToList()).ExecuteCommandAsync();
            return row > 0;
        }
        #endregion

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(Expression<Func<T, bool>> predicate)
        {
            var model = await Db.SlaveQueryable<T>().FirstAsync(predicate);
            return model;
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="order">排序字段</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(Expression<Func<T, object>> order)
        {
            var model = await Db.SlaveQueryable<T>().OrderBy(order, OrderByType.Desc).FirstAsync();
            return model;
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <param name="order">排序字段</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> order)
        {
            var model = await Db.SlaveQueryable<T>().OrderBy(order, OrderByType.Desc).FirstAsync(predicate);
            return model;
        }

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="chilProptyName">子集的属性名称</param>
        /// <param name="pidProptyName">父级id属性名</param>
        /// <param name="rootId">跟目录id（默认0）</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetTreeAsync(Expression<Func<T, IEnumerable<object>>> chilProptyName, Expression<Func<T, object>> pidProptyName, int rootId = 0)
        {
            var tree = await Db.SlaveQueryable<T>().ToTreeAsync(chilProptyName, pidProptyName, rootId);
            return tree;
        }

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="chilProptyName">子集的属性名称</param>
        /// <param name="pidProptyName">父级id属性名</param>
        /// <param name="rootId">跟目录id（默认0）</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetTreeAsync(Expression<Func<T, IEnumerable<object>>> chilProptyName, Expression<Func<T, object>> pidProptyName, Expression<Func<T, object>> primaryKeyExpression, int rootId = 0)
        {
            var tree = await Db.SlaveQueryable<T>().ToTreeAsync(chilProptyName, pidProptyName, rootId, primaryKeyExpression);
            return tree;
        }

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="chilProptyName">子集的属性名称</param>
        /// <param name="pidProptyName">父级id属性名</param>
        /// <param name="pidProptyName">主键属性名</param>
        /// <param name="rootId">跟目录id（默认0）</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetTreeAsync(string chilProptyName = "Children", string pidProptyName = "Pid", string primaryKeyPropertyName = "Id", int rootId = 0)
        {
            var tree = await Db.SlaveQueryable<T>().ToTreeAsync(chilProptyName, pidProptyName, rootId, primaryKeyPropertyName);
            return tree;
        }

        #region 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            int row = await Db.Deleteable(entity).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities">对象集合</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(IEnumerable<T> entities)
        {
            int row = await Db.Deleteable(entities.ToList()).ExecuteCommandAsync();
            return row > 0;
        }
        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id">主键</param>
        public virtual async Task<bool> DeleteAsync(object id)
        {
            int row = await Db.Deleteable<T>().In(id).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="ids">主键</param>
        public virtual async Task<bool> DeleteAsync(object[] ids)
        {
            int row = await Db.Deleteable<T>().In(ids).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="value">条件参数值(匿名对象/字典/SugarParameter)</param>
        public virtual async Task<bool> DeleteAsync(string where, object? value = null)
        {
            int row = await Db.Deleteable<T>().Where(where, value).ExecuteCommandAsync();
            return row > 0;
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">条件</param>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> where)
        {
            int row = await Db.Deleteable<T>().Where(where).ExecuteCommandAsync();
            return row > 0;
        }
        #endregion

        /// <summary>
        /// 获取数量
        /// </summary
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync()
        {
            return await Db.SlaveQueryable<T>().CountAsync();
        }

        /// <summary>
        /// 获取数量
        /// <param name="whereSql"></param>
        /// <param name="values"/>条件值(匿名对象/字典/SugarParameter)</param>
        /// </summary
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(string whereSql, object? values = null)
        {
            return await Db.SlaveQueryable<T>().Where(whereSql, values).CountAsync();
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.SlaveQueryable<T>().CountAsync(predicate);
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync()
        {
            return await Db.SlaveQueryable<T>().ToListAsync();
        }

        /// <summary>
        /// 根据lambda表达式条件获取筛选字段集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.SlaveQueryable<T>().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 根据lambda表达式条件获取筛选字段集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <param name="selector">lambda表达式条件</param>
        /// <returns></returns>
        public virtual async Task<List<A>> GetListAsync<A>(Expression<Func<T, bool>> predicate, Expression<Func<T, A>>? selector = null)
        {
            return await Db.SlaveQueryable<T>().Where(predicate).Select(selector).ToListAsync();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="whereSQL">查询条件</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(string whereSQL, object? args = null)
        {
            var result = Db.SlaveQueryable<T>();
            if (!string.IsNullOrWhiteSpace(whereSQL))
                result = result.Where(whereSQL, args);
            return await result.ToListAsync();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="selector">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<List<A>> GetListAsync<A>(string predicate, string selector = "", object? args = null)
        {
            var result = Db.SlaveQueryable<T>();
            if (!string.IsNullOrWhiteSpace(predicate))
                result = result.Where(predicate, args);
            return await result.ToList().ToDynamicListAsync<A>();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="top">前几条</param>
        /// <param name="whereSQL">查询条件</param>
        /// <param name="ordering">排序</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(int top, string whereSQL = "", string ordering = "", object? args = null)
        {
            var result = Db.SlaveQueryable<T>();

            if (!string.IsNullOrWhiteSpace(whereSQL))
                result = result.Where(whereSQL, args);

            if (!string.IsNullOrWhiteSpace(ordering))
                result = result.OrderBy(ordering);

            if (top > 0)
            {
                result = result.Take(top);
            }
            return await result.ToListAsync();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="top">前几条</param>
        /// <param name="whereSQL">查询条件</param>
        /// <param name="ordering">排序</param>
        /// <param name="selectSQL">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<List<A>> GetListAsync<A>(int top, string whereSQL = "", string ordering = "", string selectSQL = "", object? args = null)
        {
            var result = Db.SlaveQueryable<T>();

            if (!string.IsNullOrWhiteSpace(whereSQL))
                result = result.Where(whereSQL, args);

            if (!string.IsNullOrWhiteSpace(ordering))
                result = result.OrderBy(ordering);

            if (!string.IsNullOrWhiteSpace(selectSQL))
                result = result.Select(selectSQL);

            if (top > 0)
            {
                result = result.Take(top);
            }
            return await result.ToList().ToDynamicListAsync<A>();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="whereSQL">条件</param>
        /// <param name="orderSQL">排序</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<(List<T> List, int Count)> GetPageListAsync(PageModel param, string whereSQL = "", string orderSQL = "", object? args = null)
        {
            var result = Db.SlaveQueryable<T>();

            if (!string.IsNullOrWhiteSpace(whereSQL))
                result = result.Where(whereSQL, args);

            if (!string.IsNullOrWhiteSpace(orderSQL))
                result = result.OrderBy(orderSQL);
            RefAsync<int> count = 0;
            var data = await result.ToPageListAsync(param.PageIndex, param.PageSize, count);
            return (data, count);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="whereSQL">条件</param>
        /// <param name="orderSQL">排序</param>
        /// <param name="selectSQL">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<(List<A> List, int Count)> GetPageListAsync<A>(PageModel param, string whereSQL = "", string orderSQL = "", string selectSQL = "", object? args = null)
        {
            var result = Db.SlaveQueryable<T>();

            if (!string.IsNullOrWhiteSpace(whereSQL))
                result = result.Where(whereSQL, args);

            if (!string.IsNullOrWhiteSpace(orderSQL))
                result = result.OrderBy(orderSQL);

            if (!string.IsNullOrWhiteSpace(selectSQL))
                result = result.Select(selectSQL);
            RefAsync<int> count = 0;
            var data = await result.ToPageListAsync(param.PageIndex, param.PageSize, count);
            return (await data.ToDynamicListAsync<A>(), count);
        }

        /// <summary>
        /// 分页查询(条件参数0默认keyword，从1开始传)
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="whereSQL">条件</param>
        /// <param name="orderSQL">排序</param>
        /// <param name="selector">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        public virtual async Task<(List<A> List, int Count)> GetPageListAsync<A>(PageModel param, string whereSQL = "", string orderSQL = "", Expression<Func<T, A>>? selector = null, object? args = null)
        {
            var result = Db.SlaveQueryable<T>();

            if (!string.IsNullOrWhiteSpace(whereSQL))
                result = result.Where(whereSQL, args);

            if (!string.IsNullOrWhiteSpace(orderSQL))
                result = result.OrderBy(orderSQL);

            var result1 = result.Select(selector);
            RefAsync<int> count = 0;
            var data = await result1.ToPageListAsync(param.PageIndex, param.PageSize, count);
            return (await data.ToDynamicListAsync<A>(), count);
        }
        #endregion

        private static void CompareAndAssign(T original, T updated)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var oldValue = property.GetValue(original);
                var newValue = property.GetValue(updated);

                if (!Equals(oldValue, newValue))
                {
                    property.SetValue(original, newValue);
                }
            }
        }
    }
}
