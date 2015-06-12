using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using FS.Configs;
using FS.Core.Data;
using FS.Core.Infrastructure;
using FS.Mapping.Context;

namespace FS.Core
{
    public static class DbFactory
    {

        /// <summary>
        ///     创建数据库操作
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="tranLevel">开启事务等级</param>
        public static DbExecutor CreateDbExecutor<TEntity>(IsolationLevel tranLevel = IsolationLevel.Serializable) where TEntity : BaseContext
        {
            ContextMap map = typeof(TEntity);
            var dataType = map.ContextProperty.DataType;
            var connetionString = map.ContextProperty.ConnStr;
            var commandTimeout = map.ContextProperty.CommandTimeout;

            return new DbExecutor(connetionString, dataType, commandTimeout, tranLevel);
        }

        /// <summary>
        /// 返回数据库类型名称
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        public static DbProviderFactory CreateDbProviderFactory(DataBaseType dbType)
        {
            switch (dbType)
            {
                case DataBaseType.MySql: return DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                case DataBaseType.OleDb: return DbProviderFactories.GetFactory("System.Data.OleDb");
                case DataBaseType.Oracle: return DbProviderFactories.GetFactory("System.Data.OracleClient");
                case DataBaseType.SQLite: return DbProviderFactories.GetFactory("System.Data.SQLite");
                case DataBaseType.SqlServer: return DbProviderFactories.GetFactory("System.Data.SqlClient");
                //case DataBaseType.Xml: return DbProviderFactories.GetFactory("System.Linq.Xml");
            }
            return DbProviderFactories.GetFactory("System.Data.SqlClient");
        }

        /// <summary>
        /// 获取数据库连接对象
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        public static DbConnection GetDbConnection(DataBaseType dbType, string connectionString)
        {
            DbConnection conn;
            switch (dbType)
            {
                case DataBaseType.SqlServer: conn = new SqlConnection(connectionString); ; break;
                default: conn = CreateDbProviderFactory(dbType).CreateConnection(); break;
            }
            conn.ConnectionString = connectionString;
            return conn;
        }

        /// <summary>
        ///     压缩数据库
        /// </summary>
        /// <param name="dataType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        public static void Compression(string connetionString, DataBaseType dataType = DataBaseType.SqlServer)
        {
            var db = new DbExecutor(connetionString, dataType, 30);
            switch (dataType)
            {
                case DataBaseType.SQLite:
                    {
                        db.ExecuteNonQuery(CommandType.Text, "VACUUM", null);
                        break;
                    }
                default:
                    throw new NotImplementedException("该数据库不支持该方法！");
            }
        }

        /// <summary>
        ///     压缩数据库
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public static void Compression(int dbIndex)
        {
            DbInfo dbInfo = dbIndex;
            Compression(CacheManger.CreateConnString(dbIndex), dbInfo.DataType);
        }
    }
}
