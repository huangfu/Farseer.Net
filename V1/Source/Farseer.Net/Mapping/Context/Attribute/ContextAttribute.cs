using System;
using FS.Configs;
using FS.Core;
using FS.Core.Data;

namespace FS.Mapping.Context.Attribute
{
    /// <summary>
    ///     实体类的属性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    // ReSharper disable once InconsistentNaming
    public sealed class ContextAttribute : System.Attribute
    {
        /// <summary>
        ///     默认第一个数据库配置
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        public ContextAttribute(int dbIndex = 0) : this(CacheManger.CreateConnString(dbIndex), DbConfigs.ConfigEntity.DbList[dbIndex].DataType, DbConfigs.ConfigEntity.DbList[dbIndex].DataVer, DbConfigs.ConfigEntity.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        ///     默认第一个数据库配置
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dataVer">数据库版本</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        public ContextAttribute(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, string dataVer = "2008", int commandTimeout = 30)
        {
            ConnStr = connectionString;
            DataType = dbType;
            DataVer = dataVer;
            CommandTimeout = commandTimeout;
        }

        /// <summary>
        ///     设置数据库连接字符串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        ///     设置数据库类型
        /// </summary>
        public DataBaseType DataType { get; set; }

        /// <summary>
        ///     设置数据库版本
        /// </summary>
        public string DataVer { get; set; }

        /// <summary>
        ///     设置数据库执行T-SQL时间，单位秒默认是30秒
        /// </summary>
        public int CommandTimeout { get; set; }
    }
}