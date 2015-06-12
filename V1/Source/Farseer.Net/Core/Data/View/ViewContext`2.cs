using FS.Mapping.Table;

namespace FS.Core.Data.View
{
    /// <summary>
    /// 单张视图的上下文
    /// </summary>
    public class ViewContext<TPo, TVo> : ViewContext<TPo>
        where TVo : class, new()
        where TPo : ViewContext<TPo, TVo>, new()
    {
        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary> 
        protected ViewContext() { Init(); }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected ViewContext(int dbIndex) : base(dbIndex) { Init(); }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected ViewContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : base(connectionString, dbType, commandTimeout) { Init(); }

        /// <summary>
        /// 强类型实体对象
        /// </summary>
        public ViewSet<TVo> Set { get; set; }

        /// <summary>
        /// 提供快捷的数据库执行
        /// 根据实体类设置的特性，访问数据库
        /// </summary>
        public static ViewSet<TVo> Data
        {
            get
            {
                return new TPo().Set;
            }
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        private void Init()
        {
            var name = CacheManger.GetTableMap(this.GetType()).ClassInfo.Name;
            Set = new ViewSet<TVo>(this, name);
        }
    }
}