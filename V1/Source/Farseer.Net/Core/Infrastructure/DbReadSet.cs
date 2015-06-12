using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FS.Core.Data;
using FS.Extends;
using FS.Mapping.Context;
using FS.Utils;

namespace FS.Core.Infrastructure
{
    public abstract class DbReadSet<TSet, TEntity>
        where TSet : DbReadSet<TSet, TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        protected internal string Name;
        /// <summary>
        /// 实体类映射
        /// </summary>
        protected internal readonly FieldMap Map;
        /// <summary>
        /// 保存字段映射的信息
        /// </summary>
        protected internal SetState SetState;
        protected internal Queue Queue { get { return QueueManger.CreateQueue(Name, Map); } }
        protected internal abstract BaseQueueManger QueueManger { get; }

        public DbReadSet()
        {
            Map = typeof(TEntity);
        }

        #region 条件筛选器
        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public virtual TSet Select<T>(Expression<Func<TEntity, T>> select)
        {
            Queue.AddSelect(select);
            return (TSet)this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public virtual TSet Where(Expression<Func<TEntity, bool>> where)
        {
            Queue.AddWhere(where);
            return (TSet)this;
        }

        /// <summary> 自动生成o.ID == ID </summary>
        /// <param name="value">值</param>
        /// <param name="memberName">默认为主键ID属性（非数据库字段名称）</param>
        public virtual TSet Where(int value, string memberName = null)
        {
            if (string.IsNullOrWhiteSpace(memberName)) { memberName = Map.PrimaryState.Key.Name; }
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(value, memberName: memberName));
            return (TSet)this;
        }

        /// <summary> 自动生成lstIDs.Contains(o.ID) </summary>
        /// <param name="lstvValues"></param>
        /// <param name="memberName">默认为主键ID属性（非数据库字段名称）</param>
        public virtual TSet Where(List<int> lstvValues, string memberName = null)
        {
            if (string.IsNullOrWhiteSpace(memberName)) { memberName = Map.PrimaryState.Key.Name; }
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstvValues, memberName: memberName));
            return (TSet)this;
        }

        /// <summary>
        /// 倒序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="desc">字段选择器</param>
        public virtual TSet Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            Queue.AddOrderBy(desc, false);
            return (TSet)this;
        }

        /// <summary>
        /// 正序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="asc">字段选择器</param>
        public virtual TSet Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            Queue.AddOrderBy(asc, true);
            return (TSet)this;
        }
        #endregion

        #region ToTable
        /// <summary> 查询多条记录（不支持延迟加载） </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        public virtual DataTable ToTable(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.ToList(top, isDistinct, isRand).ExecuteQuery.ToTable());
        }

        /// <summary> 查询多条记录（不支持延迟加载） </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <returns></returns>
        public virtual DataTable ToTable(int pageSize, int pageIndex, bool isDistinct = false)
        {
            #region 计算总页数
            if (pageIndex < 1) { pageIndex = 1; }
            if (pageSize < 0) { pageSize = 20; }
            #endregion

            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.ToList(pageSize, pageIndex, isDistinct).ExecuteQuery.ToTable());
        }

        /// <summary> 查询多条记录（不支持延迟加载） </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="recordCount">总记录数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public virtual DataTable ToTable(int pageSize, int pageIndex, out int recordCount, bool isDistinct = false)
        {
            var queue = Queue;
            recordCount = Count();
            Queue.Copy(queue);

            #region 计算总页数
            var allCurrentPage = 1;

            if (pageIndex < 1) { pageIndex = 1; }
            if (pageSize < 0) { pageSize = 0; }
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage) { pageIndex = allCurrentPage; }
            #endregion

            return ToTable(pageSize, pageIndex, isDistinct);
        }
        #endregion

        #region ToList
        /// <summary> 查询多条记录（不支持延迟加载） </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        public virtual List<TEntity> ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            return ToTable(top, isDistinct, isRand).ToList<TEntity>();
        }

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <returns></returns>
        public virtual List<TEntity> ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            return ToTable(pageSize, pageIndex, isDistinct).ToList<TEntity>();
        }

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="recordCount">总记录数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public virtual List<TEntity> ToList(int pageSize, int pageIndex, out int recordCount, bool isDistinct = false)
        {
            return ToTable(pageSize, pageIndex, out recordCount, isDistinct).ToList<TEntity>();
        }
        #endregion

        #region ToSelectList
        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        /// <param name="select">字段选择器</param>
        public virtual List<T> ToSelectList<T>(Expression<Func<TEntity, T>> select)
        {
            return ToSelectList(0, select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="select">字段选择器</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public virtual List<T> ToSelectList<T>(int top, Expression<Func<TEntity, T>> select)
        {
            return Select(select).ToList(top).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public virtual List<T> ToSelectList<T>(List<T> lstIDs, Expression<Func<TEntity, T>> select)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            return ToSelectList(select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <param name="top">限制显示的数量</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public virtual List<T> ToSelectList<T>(List<T> lstIDs, int top, Expression<Func<TEntity, T>> select)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            return ToSelectList(top, select);
        }
        #endregion

        #region ToEntity
        /// <summary>
        /// 查询单条记录（不支持延迟加载）
        /// </summary>
        public virtual TEntity ToEntity()
        {
            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.ToEntity().ExecuteQuery.ToEntity<TEntity>());
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        public virtual TEntity ToEntity<T>(T ID)
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            return ToEntity();
        }
        #endregion

        #region Count

        /// <summary>
        /// 查询数量（不支持延迟加载）
        /// </summary>
        public virtual int Count(bool isDistinct = false, bool isRand = false)
        {
            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.Count().ExecuteQuery.ToValue<int>());
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        public virtual int Count<T>(T ID)
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            return Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public virtual int Count<T>(List<T> lstIDs)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            return Count();
        }

        #endregion

        #region IsHaving

        /// <summary>
        /// 查询数据是否存在（不支持延迟加载）
        /// </summary>
        public virtual bool IsHaving()
        {
            return Count() > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public virtual bool IsHaving<T>(T ID)
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            return IsHaving();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public virtual bool IsHaving<T>(List<T> lstIDs)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            return IsHaving();
        }

        #endregion

        #region GetValue

        /// <summary>
        /// 查询单个值（不支持延迟加载）
        /// </summary>
        public virtual T GetValue<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.GetValue().ExecuteQuery.ToValue(defValue));
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T1">ID</typeparam>
        /// <typeparam name="T2">字段类型</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <param name="fieldName">筛选字段</param>
        /// <param name="defValue">不存在时默认值</param>
        public virtual T2 GetValue<T1, T2>(T1 ID, Expression<Func<TEntity, T2>> fieldName, T2 defValue = default(T2))
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            return GetValue(fieldName, defValue);
        }

        #endregion

        #region 聚合
        /// <summary>
        /// 累计和（不支持延迟加载）
        /// </summary>
        public virtual T Sum<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Sum操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.Sum().ExecuteQuery.ToValue(defValue));
        }

        /// <summary>
        /// 查询最大数（不支持延迟加载）
        /// </summary>
        public virtual T Max<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.Max().ExecuteQuery.ToValue(defValue));
        }
        /// <summary>
        /// 查询最小数（不支持延迟加载）
        /// </summary>
        public virtual T Min<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            return QueueManger.Append(Name, Map, (queryQueue) => queryQueue.SqlBuilder.Min().ExecuteQuery.ToValue(defValue));
        }

        #endregion
    }
}
