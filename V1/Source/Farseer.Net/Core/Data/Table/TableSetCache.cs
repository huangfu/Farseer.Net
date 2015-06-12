using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FS.Extends;
using FS.Utils;

namespace FS.Core.Data.Table
{
    /// <summary>
    /// 整表数据缓存Set
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TableSetCache<TEntity> where TEntity : class, new()
    {
        private readonly TableSet<TEntity> _set;

        /// <summary>
        /// 当前缓存
        /// </summary>
        public List<TEntity> Cache { get { return CacheManger.GetSetCache<TEntity>(_set.SetState, () => _set.QueueManger.Append(_set.Name, _set.Map, (queryQueue) => queryQueue.SqlBuilder.ToList().ExecuteQuery.ToTable().ToList<TEntity>() ?? new List<TEntity>())); } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSetCache() { }
        /// <summary>
        /// 使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        public TableSetCache(TableContext context, PropertyInfo pInfo) : this(context, pInfo.Name) { }

        /// <summary>
        /// 使用属性名称的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="propertyName">属性名称</param>
        public TableSetCache(TableContext context, string propertyName)
        {
            _set = new TableSet<TEntity>(context, propertyName);

            var keyValue = _set._context.ContextMap.GetState(this.GetType(), propertyName);
            if (keyValue.Key != null) { _set.SetState = keyValue.Value; _set.Name = _set.SetState.SetAtt.Name; }
        }

        #region 条件筛选器
        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public virtual TableSetCache<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            _set.Where(where);
            return this;
        }

        /// <summary> 自动生成o.ID == ID </summary>
        /// <param name="value">值</param>
        /// <param name="memberName">默认为主键ID属性（非数据库字段名称）</param>
        public virtual TableSetCache<TEntity> Where(int value, string memberName = null)
        {
            _set.Where(value, memberName);
            return this;
        }

        /// <summary> 自动生成lstIDs.Contains(o.ID) </summary>
        /// <param name="lstvValues"></param>
        /// <param name="memberName">默认为主键ID属性（非数据库字段名称）</param>
        public virtual TableSetCache<TEntity> Where(List<int> lstvValues, string memberName = null)
        {
            _set.Where(lstvValues, memberName);
            return this;
        }
        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public virtual TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            _set.Append(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public virtual TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            _set.Append(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public virtual TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            _set.Append(fieldName, fieldValue);
            return this;
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
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpWhere;
            _set.Update(entity);

            // 更新本地缓存
            _set.QueueManger.AppendLazy(_set.Name, _set.Map, (queryQueue) =>
            {
                var lst = Cache;
                if (exp != null) { lst = lst.FindAll(exp.Compile().ToPredicate()); }

                foreach (var item in lst)
                {
                    foreach (var kic in _set.Map.MapList.Where(o => o.Value.FieldAtt.IsMap))
                    {
                        var objValue = kic.Key.GetValue(entity, null);
                        if (objValue == null || !kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(item, objValue, null);
                    }
                }
            }, !_set._context.IsMergeCommand);
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
        /// 插入（支持延迟加载）会自动赋值主键ID
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(TEntity entity)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 主键是否有值
            var indexHaveValue = _set.Map.PrimaryState.Key != null && _set.Map.PrimaryState.Key.GetValue(entity, null) != null;
            _set.Insert(entity, !indexHaveValue);

            // 更新本地缓存
            _set.QueueManger.AppendLazy(_set.Name, _set.Map, (queryQueue) => Cache.Add(entity), !_set._context.IsMergeCommand);
        }

        /// <summary>
        /// 插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"></param>
        public void Insert(List<TEntity> lst)
        {
            lst.ForEach(Insert);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除（支持延迟加载）
        /// </summary>
        public void Delete()
        {
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpWhere;
            _set.Delete();

            // 更新本地缓存
            _set.QueueManger.AppendLazy(_set.Name, _set.Map, (queryQueue) => Cache.RemoveAll(exp.Compile().ToPredicate()), !_set._context.IsMergeCommand);
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
            var assign = _set.Queue.ExpAssign;
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpWhere;
            _set.AddUp();

            // 更新本地缓存
            _set.QueueManger.AppendLazy(_set.Name, _set.Map, (queryQueue) =>
            {
                var lst = Cache;
                if (exp != null) { lst = lst.FindAll(exp.Compile().ToPredicate()); }

                foreach (var info in lst)
                {
                    foreach (var ass in assign)
                    {
                        foreach (var memberName in ass.Key.GetMemberName())
                        {
                            //获取索引的属性
                            var kic = _set.Map.MapList.FirstOrDefault(o => o.Key.Name == memberName);
                            if (kic.Key == null) { continue; }

                            var value = kic.Key.GetValue(info, null);
                            if (!kic.Key.CanWrite) { continue; }

                            object oVal;
                            switch (kic.Key.PropertyType.Name)
                            {
                                case "Int32":
                                case "Int16":
                                case "Byte": oVal = value.ConvertType(0) + ass.Value.ConvertType(0); break;
                                case "Decimal":
                                case "Long":
                                case "Float":
                                case "Double": oVal = value.ConvertType(0m) + ass.Value.ConvertType(0m); break;
                                default: throw new Exception("类型：" + kic.Key.PropertyType.Name + "， 未有转换程序对其解析。");
                            }

                            kic.Key.SetValue(info, oVal, null);
                        }
                    }

                }
            }, !_set._context.IsMergeCommand);
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
