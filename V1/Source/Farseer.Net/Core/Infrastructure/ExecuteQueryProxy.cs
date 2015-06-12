using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FS.Core.Data;
using FS.Extends;

namespace FS.Core.Infrastructure
{
    /// <summary> 将SQL发送到数据库（代理类、记录SQL、执行时间） </summary>
    public class ExecuteQueryProxy : ExecuteQuery
    {
        public ExecuteQueryProxy(Queue queue)
            : base(queue)
        {
        }

        /// <summary>
        /// 计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(Func<TReturn> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            var val = func();
            timer.Stop();

            new SqlRecordEntity(Queue.ID, Queue.Name, Queue.Sql, Queue.Param).Write(timer.ElapsedMilliseconds);
            return val;
        }

        public override int Execute()
        {
            return SpeedTest(() => base.Execute());
        }
        public override DataTable ToTable()
        {
            return SpeedTest(() => base.ToTable());
        }
        public override TEntity ToEntity<TEntity>()
        {
            return SpeedTest(() => base.ToEntity<TEntity>());
        }
        public override T ToValue<T>(T defValue = default(T))
        {
            return SpeedTest(() => base.ToValue(defValue));
        }
        public override int Execute<TEntity>(TEntity entity)
        {
            return SpeedTest(() => base.Execute(entity));
        }
        public override List<TEntity> ToList<TEntity>(TEntity entity)
        {
            return SpeedTest(() => base.ToList(entity));
        }
        public override TEntity ToEntity<TEntity>(TEntity entity)
        {
            return SpeedTest(() => base.ToEntity(entity));
        }
        public override T ToValue<TEntity, T>(TEntity entity, T defValue = default(T))
        {
            return SpeedTest(() => base.ToValue(entity, defValue));
        }
    }

    /// <summary> 记录SQL执行情况 </summary>
    [Serializable]
    public class SqlRecordEntity
    {
        public SqlRecordEntity() { }
        public SqlRecordEntity(Guid id, string name, StringBuilder sql, List<DbParameter> param)
        {
            ID = id.ToString();
            Name = name;
            SqlParamList = new List<SqlParam>();
            Sql = sql != null ? sql.ToString() : "";
            if (param != null && param.Count > 0) { param.ForEach(o => SqlParamList.Add(new SqlParam { Name = o.ParameterName, Value = o.Value.ToString() })); }
        }
        /// <summary> 当前队列的ID </summary>
        public string ID { get; set; }
        /// <summary> 执行时间（毫秒） </summary>
        public long UserTime { get; set; }
        /// <summary> 执行表名称 </summary>
        public string Name { get; set; }
        /// <summary> 执行行数 </summary>
        public int LineNo { get; set; }
        /// <summary> 执行方法名称 </summary>
        public string MethodName { get; set; }
        /// <summary> 执行方法的文件名 </summary>
        public string FileName { get; set; }
        /// <summary> 执行时间 </summary>
        public DateTime CreateAt { get; set; }
        /// <summary> 执行SQL </summary>
        public string Sql { get; set; }
        /// <summary> 执行参数 </summary>
        public List<SqlParam> SqlParamList { get; set; }

        /// <summary> 写入~/App_Data/SqlLog.xml </summary>
        /// <param name="elapsedMilliseconds">执行耗时</param>
        public void Write(long elapsedMilliseconds)
        {
            CreateAt = DateTime.Now;
            UserTime = elapsedMilliseconds;

            var lstFrames = new StackTrace(true).GetFrames();
            if (lstFrames == null) return;
            var stack = lstFrames.LastOrDefault(o => o.GetFileLineNumber() != 0 && !o.GetMethod().Module.Name.IsEquals("Farseer.Net.dll") && !o.GetMethod().Name.IsEquals("Callback"));
            if (stack == null) return;

            LineNo = stack.GetFileLineNumber();
            MethodName = stack.GetMethod().Name;
            FileName = stack.GetFileName();

            // 序列化
            CacheManger.AddSqlRecord(this);
        }
    }

    public class SqlParam
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
