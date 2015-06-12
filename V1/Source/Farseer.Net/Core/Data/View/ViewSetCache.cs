using System.Collections.Generic;
using System.Reflection;
using FS.Extends;

namespace FS.Core.Data.View
{
    /// <summary>
    /// 整表数据缓存Set
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ViewSetCache<TEntity> where TEntity : class, new()
    {
        private readonly ViewSet<TEntity> _set;

        /// <summary>
        /// 当前缓存
        /// </summary>
        public List<TEntity> Cache { get { return CacheManger.GetSetCache<TEntity>(_set.SetState, () => _set.QueueManger.Append(_set.Name, _set.Map, (queryQueue) => queryQueue.SqlBuilder.ToList().ExecuteQuery.ToTable().ToList<TEntity>() ?? new List<TEntity>())); } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ViewSetCache() { }
        /// <summary>
        /// 使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        public ViewSetCache(ViewContext context, PropertyInfo pInfo) : this(context, pInfo.Name) { }

        /// <summary>
        /// 使用属性名称的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="propertyName">属性名称</param>
        public ViewSetCache(ViewContext context, string propertyName)
        {
            _set = new ViewSet<TEntity>(context, propertyName);

            var keyValue = _set._context.ContextMap.GetState(this.GetType(), propertyName);
            if (keyValue.Key != null) { _set.SetState = keyValue.Value; _set.Name = _set.SetState.SetAtt.Name; }
        }
    }
}
