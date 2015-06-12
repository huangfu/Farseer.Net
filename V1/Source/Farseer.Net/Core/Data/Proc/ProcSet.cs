using System.Collections.Generic;
using System.Reflection;
using FS.Mapping.Context;

namespace FS.Core.Data.Proc
{
    /// <summary>
    /// 存储过程操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ProcSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly ProcContext _context;

        private ProcQueueManger QueueManger { get { return _context.QueueManger; } }

        /// <summary>
        /// 存储过程名
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 实体类映射
        /// </summary>
        private readonly FieldMap _map;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ProcSet() { }

        /// <summary>
        /// 使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        public ProcSet(ProcContext context, PropertyInfo pInfo) : this(context, pInfo.Name) { }

        /// <summary>
        /// 使用属性名称的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="propertyName">属性名称</param>
        public ProcSet(ProcContext context, string propertyName)
        {
            _context = context;
            _map = typeof(TEntity);
            var contextState = _context.ContextMap.GetState(this.GetType(), propertyName);
            _name = contextState.Value.SetAtt.Name;
        }

        /// <summary>
        /// 返回查询的值
        /// </summary>
        public T GetValue<T>(TEntity entity = null, T t = default(T))
        {
            // 加入委托
            return QueueManger.Append(_name, _map, (queryQueue) => queryQueue.ExecuteQuery.ToValue(entity, t));
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        public void Execute(TEntity entity = null)
        {
            // 加入委托
            QueueManger.AppendLazy(_name, _map, (queryQueue) => queryQueue.ExecuteQuery.Execute(entity), !_context.IsMergeCommand);
        }

        /// <summary>
        /// 返回单条记录
        /// </summary>
        public TEntity ToEntity(TEntity entity = null)
        {
            // 加入委托
            return QueueManger.Append(_name, _map, (queryQueue) => entity = queryQueue.ExecuteQuery.ToEntity(entity));
        }

        /// <summary>
        /// 返回多条记录
        /// </summary>
        public List<TEntity> ToList(TEntity entity = null)
        {
            // 加入委托
            return QueueManger.Append(_name, _map, (queryQueue) => queryQueue.ExecuteQuery.ToList(entity));
        }
    }
}