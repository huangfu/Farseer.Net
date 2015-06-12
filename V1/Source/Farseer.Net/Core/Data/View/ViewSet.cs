using System.Reflection;
using FS.Core.Infrastructure;

namespace FS.Core.Data.View
{
    /// <summary>
    /// 视图操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ViewSet<TEntity> : DbReadSet<ViewSet<TEntity>, TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        internal readonly ViewContext _context;
        protected internal override BaseQueueManger QueueManger { get { return _context.QueueManger; } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ViewSet() { }

        /// <summary>
        /// 使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        public ViewSet(ViewContext context, PropertyInfo pInfo) : this(context, pInfo.Name) { }
        /// <summary>
        /// 使用属性名称的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="propertyName">属性名称</param>
        public ViewSet(ViewContext context, string propertyName)
        {
            _context = context;

            var keyValue = _context.ContextMap.GetState(this.GetType(), propertyName);
            if (keyValue.Key != null) { SetState = keyValue.Value; Name = SetState.SetAtt.Name; }
        }
    }
}