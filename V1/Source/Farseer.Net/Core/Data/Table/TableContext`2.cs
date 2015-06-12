using FS.Mapping.Table;
using System.ComponentModel;

namespace FS.Core.Data.Table
{
    /// <summary>
    /// 单张表的上下文
    /// </summary>
    public class TableContext<TPo, TVo> : TableContext<TPo>
        where TVo : class, new()
        where TPo : TableContext<TPo, TVo>, new()
    {
        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        protected TableContext() { Init(); }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected TableContext(int dbIndex) : base(dbIndex) { Init(); }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected TableContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : base(connectionString, dbType, commandTimeout) { Init(); }

        /// <summary>
        /// 强类型实体对象
        /// </summary>
        public TableSet<TVo> Set { get; private set; }

        /// <summary>
        /// 提供快捷的数据库执行
        /// 根据实体类设置的特性，访问数据库
        /// </summary>
        public static TableSet<TVo> Data
        {
            get
            {
                return new TPo() { IsMergeCommand = false }.Set;
            }
        }
        /// <summary>
        /// 静态实例
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new static TPo Instance { get; set; }

        /// <summary>
        /// 设置表名
        /// </summary>
        private void Init()
        {
            var name = CacheManger.GetTableMap(this.GetType()).ClassInfo.Name;
            Set = new TableSet<TVo>(this, name);
        }
    }
}