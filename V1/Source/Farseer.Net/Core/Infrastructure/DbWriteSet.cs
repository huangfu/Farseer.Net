using System;
using System.Linq.Expressions;

namespace FS.Core.Infrastructure
{
    public abstract class DbWriteSet<TSet, TEntity> : DbReadSet<TSet, TEntity>
        where TSet : DbWriteSet<TSet, TEntity>
        where TEntity : class, new()
    {

        #region 条件
        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public virtual TSet Append<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            Queue.AddAssign(fieldName, fieldValue);
            return (TSet)this;
        }

        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public virtual TSet Append<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            Queue.AddAssign(fieldName, fieldValue);
            return (TSet)this;
        }

        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public virtual TSet Append<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            Queue.AddAssign(fieldName, fieldValue);
            return (TSet)this;
        }
        #endregion
    }
}
