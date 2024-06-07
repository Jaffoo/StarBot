using System.Linq.Expressions;
using System.Security.Cryptography;
using PageModel = StarBot.Model.PageModel;

namespace StarBot.Repository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate);

        #region 新增

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> AddAsync(T entity);

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        Task<int> AddAsync(T entity, params string[] insterCol);

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        Task<int> AddAsync(T entity, Expression<Func<T, object>>? insterCol = null);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns></returns>
        Task<bool> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        Task<bool> AddRangeAsync(IEnumerable<T> entities, params string[] insterCol);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="insterCol">插入指定字段</param>
        /// <returns></returns>
        Task<bool> AddRangeAsync(IEnumerable<T> entities, Expression<Func<T, object>>? insterCol = null);
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="newEntity">老实体</param>
        /// <param name="oldEntity">新实体</param>
        Task<bool> UpdateAsync(T newEntity, T oldEntity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="newEntities">老实体</param>
        /// <param name="oldEntities">新实体</param>
        Task<bool> UpdateAsync(IEnumerable<T> newEntities, IEnumerable<T> oldEntities);


        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="updateCol">要更新的字段</param>
        Task<bool> UpdateAsync(T entity, params string[] updateCol);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="updateCol">要更新的字段</param>
        Task<bool> UpdateAsync(T entity, Expression<Func<T, object>>? updateCol = null);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        Task<bool> UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="updateCol">要更新的字段</param>
        Task<bool> UpdateRangeAsync(IEnumerable<T> entities, params string[] updateCol);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="updateCol">要更新的字段</param>
        Task<bool> UpdateRangeAsync(IEnumerable<T> entities, Expression<Func<T, object>>? updateCol = null);

        #endregion

        #region 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        Task<bool> DeleteAsync(T entity);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities">对象集合</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(IEnumerable<T> entities);
        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id">主键</param>
        Task<bool> DeleteAsync<Tid>(Tid id) where Tid : struct;

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="ids">主键</param>
        Task<bool> DeleteAsync<Tid>(IEnumerable<Tid> ids) where Tid : struct;

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="value">条件参数(匿名对象/字典/SugarParameter)</param>
        Task<bool> DeleteAsync(string where, object? value = null);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">条件</param>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> where);
        #endregion

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync();

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="whereSql">条件</param>
        /// <param name="values">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<int> GetCountAsync(string whereSql, object? values = null);

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetListAsync();

        /// <summary>
        /// 获取实体集
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(IEnumerable<int> ids);

        /// <summary>
        /// 根据lambda表达式条件获取筛选字段集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据lambda表达式条件获取筛选字段集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <param name="selector">lambda表达式条件</param>
        /// <returns></returns>
        Task<List<A>> GetListAsync<A>(Expression<Func<T, bool>> predicate, Expression<Func<T, A>>? selector = null);

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="whereSQL">条件SQL</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(string whereSQL, object? args = null);

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="whereSQL">条件SQL</param>
        /// <param name="selectSQL">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<List<A>> GetListAsync<A>(string whereSQL, string selectSQL = "", object? args = null);

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="top">前几条</param>
        /// <param name="whereSQL">条件SQL</param>
        /// <param name="ordering">排序</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(int top, string whereSQL = "", string ordering = "", object? args = null);


        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="top">前几条</param>
        /// <param name="whereSQL">条件SQL</param>
        /// <param name="ordering">排序</param>
        /// <param name="selectSQL">筛选字段SQL</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<List<A>> GetListAsync<A>(int top, string whereSQL = "", string ordering = "", string selectSQL = "", object? args = null);

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">lambda表达式条件</param>
        /// <returns></returns>
        Task<T> GetModelAsync(int id);

        /// <summary>
        /// 根据lambda表达式条件获取第一个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        Task<T> GetModelAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据lambda表达式条件获取最后一个实体
        /// </summary>
        /// <param name="order">排序字段表达式</param>
        /// <returns></returns>
        Task<T> GetModelAsync(Expression<Func<T, object>> order);

        /// <summary>
        /// 根据lambda表达式条件获取最后一个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <param name="order">排序字段表达式</param>
        /// <returns></returns>
        Task<T> GetModelAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> order);

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="chilProptyName">子集的属性名称</param>
        /// <param name="pidProptyName">父级id属性名</param>
        /// <param name="rootId">跟目录id（默认0）</param>
        /// <returns></returns>
        Task<List<T>> GetTreeAsync(Expression<Func<T, IEnumerable<object>>> chilProptyName, Expression<Func<T, object>> pidProptyName, int rootId = 0);

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="chilProptyName">子集的属性名称</param>
        /// <param name="pidProptyName">父级id属性名</param>
        /// <param name="rootId">跟目录id（默认0）</param>
        /// <returns></returns>
        Task<List<T>> GetTreeAsync(Expression<Func<T, IEnumerable<object>>> chilProptyName, Expression<Func<T, object>> pidProptyName, Expression<Func<T, object>> primaryKeyExpression, int rootId = 0);

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="chilProptyName">子集的属性名称</param>
        /// <param name="pidProptyName">父级id属性名</param>
        /// <param name="rootId">跟目录id（默认0）</param>
        /// <returns></returns>
        Task<List<T>> GetTreeAsync(string chilProptyName = "Children", string pidProptyName = "Pid", string primaryKeyPropertyName = "Id", int rootId = 0);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="whereSQL">条件</param>
        /// <param name="orderSQL">排序</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<(List<T> List, int Count)> GetPageListAsync(PageModel param, string whereSQL = "", string orderSQL = "", object? args = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="whereSQL">条件</param>
        /// <param name="orderSQL">排序</param>
        /// <param name="selectSQL">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<(List<A> List, int Count)> GetPageListAsync<A>(PageModel param, string whereSQL = "", string orderSQL = "", string selectSQL = "", object? args = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="whereSQL">条件</param>
        /// <param name="orderSQL">排序</param>
        /// <param name="selector">筛选字段</param>
        /// <param name="args">条件参数(匿名对象/字典/SugarParameter)</param>
        /// <returns></returns>
        Task<(List<A> List, int Count)> GetPageListAsync<A>(PageModel param, string whereSQL = "", string orderSQL = "", Expression<Func<T, A>>? selector = null, object? args = null);
    }
}
