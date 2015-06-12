using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FS.Core.Infrastructure;
using FS.Utils;

namespace FS.Core.Data.Table
{
    /// <summary>
    /// 表操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TableSet<TEntity> : DbWriteSet<TableSet<TEntity>, TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        internal readonly TableContext _context;
        protected internal override BaseQueueManger QueueManger { get { return _context.QueueManger; } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }
        /// <summary>
        /// 使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        public TableSet(TableContext context, PropertyInfo pInfo) : this(context, pInfo.Name) { }
        /// <summary>
        /// 使用属性名称的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="propertyName">属性名称</param>
        public TableSet(TableContext context, string propertyName)
        {
            _context = context;
            var keyValue = _context.ContextMap.GetState(this.GetType(), propertyName);
            if (keyValue.Key != null) { SetState = keyValue.Value; Name = SetState.SetAtt.Name; }

        }

        #region Copy

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity">对新职的赋值</param>
        public void Copy(Action<TEntity> acTEntity = null)
        {
            var lst = ToList();
            foreach (var info in lst)
            {
                if (acTEntity != null) acTEntity(info);
                Insert(info);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public void Copy<T>(T ID, Action<TEntity> act = null)
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            Copy(act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        public void Copy<T>(List<T> lstIDs, Action<TEntity> act = null)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            Copy(act);
        }

        #endregion

        #region Update

        /// <summary>
        /// 修改（支持延迟加载）
        /// 如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"></param>
        public void Update(TEntity entity)
        {
            Check.NotNull(entity, "更新操作时，参数不能为空！");

            // 加入委托
            QueueManger.AppendLazy(Name, Map, (queryQueue) => queryQueue.SqlBuilder.Update(entity).ExecuteQuery.Execute(), !_context.IsMergeCommand);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public void Update<T>(TEntity info, T ID)
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            Update(info);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public void Update<T>(TEntity info, List<T> lstIDs)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            Update(info);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="where">查询条件</param>
        public void Update(TEntity info, Expression<Func<TEntity, bool>> where)
        {
            Where(where).Update(info);
        }
        #endregion

        #region Insert

        /// <summary>
        /// 插入（支持延迟加载）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isReturnLastID">是否设置主键ID为返回的最新ID</param>
        public void Insert(TEntity entity, bool isReturnLastID = false)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 加入委托
            QueueManger.AppendLazy(Name, Map, (queryQueue) =>
            {
                if (!isReturnLastID) { queryQueue.SqlBuilder.Insert(entity).ExecuteQuery.Execute(); }
                else
                {
                    // 返回主键ID
                    var ident = queryQueue.SqlBuilder.InsertIdentity(entity).ExecuteQuery.ToValue<int>();
                    // 设置主键ID
                    if (Map.PrimaryState.Key != null) { Map.PrimaryState.Key.SetValue(entity, ident, null); }
                }
            }, !_context.IsMergeCommand);
        }

        /// <summary>
        /// 插入（延迟加载情况下，ID只有在SaveChange()后才能返回）
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="identity">返回新增的</param>
        public void Insert(TEntity entity, out int identity)
        {
            if (entity == null) { throw new ArgumentNullException("entity", "插入操作时，参数不能为空！"); }

            var ident = 0;
            QueueManger.AppendLazy(Name, Map, (queryQueue) => ident = queryQueue.SqlBuilder.InsertIdentity(entity).ExecuteQuery.ToValue<int>(), !_context.IsMergeCommand);
            identity = ident;
        }

        /// <summary>
        /// 插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"></param>
        public void Insert(List<TEntity> lst)
        {
            if (lst == null) { throw new ArgumentNullException("lst", "插入操作时，lst参数不能为空！"); }

            // 加入委托
            QueueManger.AppendLazy(Name, Map, (queryQueue) =>
            {
                // 如果是MSSQLSER，则启用BulkCopy
                if (QueueManger.DataBase.DataType == DataBaseType.SqlServer) { QueueManger.DataBase.ExecuteSqlBulkCopy(Name, ConvertHelper.ToTable(lst)); }
                else { lst.ForEach(entity => queryQueue.SqlBuilder.Insert(entity).ExecuteQuery.Execute()); }
            }, !_context.IsMergeCommand);

        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除（支持延迟加载）
        /// </summary>
        public void Delete()
        {
            // 加入委托
            QueueManger.AppendLazy(Name, Map, (queryQueue) => queryQueue.SqlBuilder.Delete().ExecuteQuery.Execute(), !_context.IsMergeCommand);
        }
        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        public void Delete<T>(T ID)
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            Delete();
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        public void Delete<T>(List<T> lstIDs)
        {
            Where(ConvertHelper.CreateContainsBinaryExpression<TEntity>(lstIDs));
            Delete();
        }
        #endregion

        #region AddUp
        /// <summary>
        /// 添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public void AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            Append(fieldName, fieldValue).AddUp();
        }

        /// <summary>
        /// 添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public void AddUp<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue)
            where T : struct
        {
            Append(fieldName, fieldValue).AddUp();
        }
        /// <summary>
        /// 添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public void AddUp()
        {
            if (Queue.ExpAssign == null) { throw new ArgumentNullException("ExpAssign", "+=字段操作时，必须先执行AddUp的另一个重载版本！"); }

            // 加入委托
            QueueManger.AppendLazy(Name, Map, (queryQueue) => queryQueue.SqlBuilder.AddUp().ExecuteQuery.Execute(), !_context.IsMergeCommand);

        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public void AddUp<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue) where T : struct
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            AddUp(select, fieldValue);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public void AddUp<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue)
            where T : struct
        {
            Where(ConvertHelper.CreateBinaryExpression<TEntity>(ID));
            AddUp(select, fieldValue);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue">要更新的值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public void AddUp<T>(T? ID, T fieldValue)
            where T : struct
        {
            AddUp<T>(ID, null, fieldValue);
        }
        #endregion
    }
}
