using System.ComponentModel;
using FS.Core.Infrastructure;

namespace FS.Core.Data.View
{
    /// <summary>
    /// 多张视图上下文
    /// </summary>
    public class ViewContext : BaseContext
    {
        /// <summary>
        /// 使用DB特性设置数据库信息
        /// </summary>
        protected ViewContext()
        {
            InstanceProperty();
        }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected ViewContext(int dbIndex) : base(dbIndex) { InstanceProperty(); }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected ViewContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : base(connectionString, dbType, commandTimeout) { InstanceProperty(); }

        /// <summary>
        /// 队列管理
        /// </summary>
        protected internal ViewQueueManger QueueManger { get; private set; }

        /// <summary>
        /// 实例化子类中，所有Set属性
        /// </summary>
        private void InstanceProperty()
        {
            QueueManger = new ViewQueueManger(DataBase, ContextMap);
            InstanceProperty(this, "ViewSet`1");
            InstanceProperty(this, "ViewSetCache`1");
        }


        /// <summary>
        /// 动态返回TableSet类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public ViewSet<TEntity> Set<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo<TEntity>(typeof(ViewSet<TEntity>), propertyName);
            // 找到存在的属性后，返回属性
            return new ViewSet<TEntity>(this, pInfo);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.Dispose();
            }
        }
    }
}