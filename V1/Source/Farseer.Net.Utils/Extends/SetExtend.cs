using FS.Core.Data.Table;
using FS.Core.Data.View;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class SetExtend
    {
        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToNextEntity<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID > ID).Asc(o => o.ID).ToEntity();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToPreviousEntity<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID < ID).Desc(o => o.ID).ToEntity();
        }
        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToNextEntity<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID > ID).Asc(o => o.ID).ToEntity();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToPreviousEntity<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID < ID).Desc(o => o.ID).ToEntity();
        }
    }
}