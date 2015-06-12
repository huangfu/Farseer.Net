using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading;
using FS.Configs;
using FS.Core.Infrastructure;
using FS.Mapping.Context;
using FS.Mapping.Verify;
using FS.Utils;

namespace FS.Core
{
    /// <summary>
    /// 框架缓存管理
    /// </summary>
    public abstract class CacheManger
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        #region Context映射的信息
        /// <summary>
        ///     缓存所有实体类
        /// </summary>
        private static readonly Dictionary<Type, ContextMap> ContextMapList = new Dictionary<Type, ContextMap>();
        /// <summary>
        ///     返回Context映射的信息
        /// </summary>
        /// <param name="type">实体类</param>
        public static ContextMap GetContextMap(Type type)
        {
            if (ContextMapList.ContainsKey(type)) return ContextMapList[type];
            lock (LockObject)
            {
                if (!ContextMapList.ContainsKey(type))
                {
                    ContextMapList.Add(type, new ContextMap(type));
                }
            }

            return ContextMapList[type];
        }
        #endregion

        #region Field映射的信息
        /// <summary>
        ///     缓存所有实体类
        /// </summary>
        private static readonly Dictionary<Type, FieldMap> FieldMapList = new Dictionary<Type, FieldMap>();
        /// <summary>
        ///     返回Field映射的信息
        /// </summary>
        /// <param name="type">实体类</param>
        public static FieldMap GetFieldMap(Type type)
        {
            if (FieldMapList.ContainsKey(type)) return FieldMapList[type];
            lock (LockObject)
            {
                if (!FieldMapList.ContainsKey(type))
                {
                    FieldMapList.Add(type, new FieldMap(type));
                }
            }

            return FieldMapList[type];
        }
        #endregion

        #region 验证的实体类映射的信息
        /// <summary>
        ///     缓存所有验证的实体类
        /// </summary>
        private static readonly Dictionary<Type, VerifyMap> VerifyMapList = new Dictionary<Type, VerifyMap>();
        /// <summary>
        ///     返回验证的实体类映射的信息
        /// </summary>
        /// <param name="type">IVerification实体类</param>
        public static VerifyMap GetVerifyMap(Type type)
        {
            if (VerifyMapList.ContainsKey(type)) return VerifyMapList[type];
            lock (LockObject)
            {
                if (!VerifyMapList.ContainsKey(type))
                {
                    VerifyMapList.Add(type, new VerifyMap(type));
                }
            }

            return VerifyMapList[type];
        }
        #endregion

        #region 数据库连接字符串
        /// <summary>
        /// 连接字符串缓存
        /// </summary>
        private static readonly Dictionary<int, string> ConnList = new Dictionary<int, string>();
        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public static string CreateConnString(int dbIndex = 0)
        {
            if (ConnList.ContainsKey(dbIndex)) return ConnList[dbIndex];
            lock (LockObject)
            {
                if (ConnList.ContainsKey(dbIndex)) return ConnList[dbIndex];

                DbInfo dbInfo = dbIndex;
                ConnList.Add(dbIndex, DbProvider.CreateInstance(dbInfo.DataType).CreateDbConnstring(dbInfo.UserID, dbInfo.PassWord, dbInfo.Server, dbInfo.Catalog, dbInfo.DataVer, dbInfo.ConnectTimeout, dbInfo.PoolMinSize, dbInfo.PoolMaxSize, dbInfo.Port));
            }

            return ConnList[dbIndex];


        }
        #endregion

        #region 实体数据缓存
        /// <summary>
        /// 实体数据缓存
        /// </summary>
        private static readonly Dictionary<SetState, IList> SetCache = new Dictionary<SetState, IList>();
        /// <summary>
        /// 获取实体数据缓存
        /// </summary>
        /// <param name="setState"></param>
        /// <param name="initCache">不存在数据时，初始化操作</param>
        /// <returns></returns>
        public static List<TEntity> GetSetCache<TEntity>(SetState setState, Func<IList> initCache = null)
        {
            return (List<TEntity>)GetSetCache(setState, initCache);
        }
        /// <summary>
        /// 获取实体数据缓存
        /// </summary>
        /// <param name="setState"></param>
        /// <param name="initCache">不存在数据时，初始化操作</param>
        /// <returns></returns>
        public static IList GetSetCache(SetState setState, Func<IList> initCache = null)
        {
            if (SetCache.ContainsKey(setState)) { return SetCache[setState]; }
            if (initCache == null) { return null; }

            lock (LockObject)
            {
                if (!SetCache.ContainsKey(setState)) { SetCache.Add(setState, initCache()); }
            }
            return SetCache[setState];
        }
        #endregion

        #region 枚举的Display.Name
        /// <summary>
        ///     枚举缓存列表
        /// </summary>
        private static readonly Dictionary<string, string> EnumList = new Dictionary<string, string>();
        /// <summary>
        ///     返回枚举的Display.Name
        /// </summary>
        /// <param name="eum">枚举</param>
        public static string GetEnumName(Enum eum)
        {
            if (eum == null) { return ""; }
            var enumType = eum.GetType();
            var enumName = eum.ToString();
            var key = string.Format("{0}.{1}", enumType.FullName, enumName);

            if (EnumList.ContainsKey(key)) { return EnumList[key]; }

            foreach (var fieldInfo in enumType.GetFields())
            {
                //判断名称是否相等   
                if (fieldInfo.Name != enumName) continue;

                //反射出自定义属性   
                foreach (Attribute attr in fieldInfo.GetCustomAttributes(true))
                {
                    //类型转换找到一个Description，用Description作为成员名称
                    var dscript = attr as DisplayAttribute;
                    if (dscript == null) { continue; }
                    lock (LockObject)
                    {
                        if (!EnumList.ContainsKey(key)) { EnumList.Add(key, dscript.Name); }
                    }
                    return dscript.Name;
                }
            }

            //如果没有检测到合适的注释，则用默认名称   
            return enumName;
        }
        #endregion

        #region Sql记录日志
        /// <summary>
        /// SQL日志保存定时器
        /// </summary>
        private static Timer _saveSqlRecord;

        private static List<SqlRecordEntity> _sqlRecordList;
        /// <summary>
        /// 获取当前SQL日志
        /// </summary>
        /// <returns></returns>
        public static List<SqlRecordEntity> GetSqlRecord()
        {
            if (_sqlRecordList != null) { return _sqlRecordList; }

            var path = SysMapPath.AppData;
            const string fileName = "SqlLog.xml";
            return (_sqlRecordList = Serialize.Load<List<SqlRecordEntity>>(path, fileName) ?? new List<SqlRecordEntity>());
        }
        /// <summary>
        /// 添加SQL日志，并启动定时保存
        /// </summary>
        /// <param name="entity"></param>
        public static void AddSqlRecord(SqlRecordEntity entity)
        {
            lock (LockObject) { GetSqlRecord(); }

            _sqlRecordList.Add(entity);
            // 启动10秒后保存
            if (_saveSqlRecord == null)
            {
                _saveSqlRecord = new Timer(o =>
                    {
                        var path = SysMapPath.AppData;
                        const string fileName = "SqlLog.xml";
                        Serialize.Save(_sqlRecordList, path, fileName);
                        _saveSqlRecord.Dispose();
                        _saveSqlRecord = null;
                    }, null, 1000 * 1, -1);
            }
        }
        #endregion

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void ClearCache()
        {
            ContextMapList.Clear();
            FieldMapList.Clear();
            VerifyMapList.Clear();
            ConnList.Clear();
            _sqlRecordList.Clear();
        }
    }
}
