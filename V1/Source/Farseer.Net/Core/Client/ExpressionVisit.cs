using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer;
using FS.Core.Data;
using FS.Core.Data.Table;
using FS.Core.Infrastructure;
using FS.Mapping.Context.Attribute;

namespace FS.Core.Client
{
    /// <summary>
    /// 数据库字段解析器总入口，根据要解析的类型，再分散到各自负责的解析器
    /// </summary>
    public class ExpressionVisit
    {
        /// <summary>
        /// 提供ExpressionNew表达式树的解析
        /// </summary>
        private readonly DbExpressionNewProvider _expNewProvider;
        /// <summary>
        /// 提供ExpressionBinary表达式树的解析
        /// </summary>
        private readonly DbExpressionBoolProvider _expBoolProvider;

        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly BaseQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly Queue Queue;

        /// <summary>
        /// 禁止无参数构造器
        /// </summary>
        private ExpressionVisit() { }

        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public ExpressionVisit(BaseQueueManger queueManger, Queue queue)
        {
            QueueManger = queueManger;
            Queue = queue;

            _expNewProvider = new ExpressionNew(QueueManger, Queue);
            _expBoolProvider = new ExpressionBool(QueueManger, Queue);
        }

        /// <summary>
        /// 主键如果有值、或者设置成只读条件，则自动转成条件
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="entity">实体类</param>
        public string ReadCondition<TEntity>(TEntity entity) where TEntity : class, new()
        {
            var strWhereSql = new StringBuilder();
            // 主键如果有值、或者设置成只读条件，则自动转成条件
            foreach (var source in Queue.FieldMap.MapList.Where(o => o.Value.FieldAtt.IsPrimaryKey || o.Value.FieldAtt.UpdateStatusType == StatusType.ReadCondition))
            {
                var value = source.Key.GetValue(entity, null);
                if (value == null) { continue; }

                if (strWhereSql.Length > 0) { strWhereSql.Append(" AND "); }
                var param = QueueManger.DbProvider.CreateDbParam("ReadCondition_" + source.Value.FieldAtt.Name, value);
                strWhereSql.Append(string.Format("{0} = {1}", source.Value.FieldAtt.Name, param.ParameterName));
                Queue.Param.Add(param);
            }
            return strWhereSql.ToString();
        }

        /// <summary>
        /// 赋值解析器
        /// </summary>
        /// <param name="entity">实体类</param>
        public string Assign<TEntity>(TEntity entity) where TEntity : class,new()
        {
            Clear();

            var map = CacheManger.GetFieldMap(typeof(TEntity));
            var sb = new StringBuilder();

            //  迭代实体赋值情况
            foreach (var kic in map.MapList.Where(o => o.Value.FieldAtt.IsMap && o.Value.FieldAtt.UpdateStatusType == StatusType.CanWrite))
            {
                // 如果主键有值，则取消修改主键的SQL
                if (kic.Value.FieldAtt.IsPrimaryKey) { continue; }
                var obj = kic.Key.GetValue(entity, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                //  查找组中是否存在已有的参数，有则直接取出
                var newParam = QueueManger.DbProvider.CreateDbParam(Queue.Index + "_" + kic.Value.FieldAtt.Name, obj);
                Queue.Param.Add(newParam);

                //  添加参数到列表
                sb.AppendFormat("{0} = {1} ,", QueueManger.DbProvider.KeywordAegis(kic.Key.Name), newParam.ParameterName);
            }

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 赋值解析器
        /// </summary>
        /// <param name="exp">单个字段的赋值</param>
        public string Assign(Expression exp)
        {
            Clear();
            if (exp == null) { return null; }
            var sb = new StringBuilder();
            _expNewProvider.Visit(exp, false);
            _expNewProvider.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 插入字段解析器
        /// </summary>
        /// <param name="entity">实体类</param>
        public string Insert<TEntity>(TEntity entity) where TEntity : class,new()
        {
            Clear();

            var map = CacheManger.GetFieldMap(typeof(TEntity));
            //  字段
            var strFields = new StringBuilder();
            //  值
            var strValues = new StringBuilder();

            //  迭代实体赋值情况
            foreach (var kic in map.MapList.Where(o => o.Value.FieldAtt.IsMap && o.Value.FieldAtt.InsertStatusType == StatusType.CanWrite))
            {
                var obj = kic.Key.GetValue(entity, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                //  查找组中是否存在已有的参数，有则直接取出
                var newParam = QueueManger.DbProvider.CreateDbParam(Queue.Index + "_" + kic.Value.FieldAtt.Name, obj);
                Queue.Param.Add(newParam);

                //  添加参数到列表
                strFields.AppendFormat("{0},", QueueManger.DbProvider.KeywordAegis(kic.Key.Name));
                strValues.AppendFormat("{0},", newParam.ParameterName);
            }
            //QueryQueue.Param = lstParam;
            return "(" + strFields.Remove(strFields.Length - 1, 1) + ") VALUES (" + strValues.Remove(strValues.Length - 1, 1) + ")";
        }
        /// <summary>
        /// 排序解析器
        /// </summary>
        /// <param name="lstExp">多个排序字段(true:正序；false：倒序）</param>
        /// <returns></returns>
        public string OrderBy(Dictionary<Expression, bool> lstExp)
        {
            if (lstExp == null || lstExp.Count == 0) { return null; }
            var sb = new StringBuilder();
            foreach (var keyValue in lstExp)
            {
                Clear();
                _expNewProvider.Visit(keyValue.Key, false);
                _expNewProvider.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
                if (sb.Length <= 0) continue;
                sb = sb.Remove(sb.Length - 1, 1).Append(string.Format(" {0}", keyValue.Value ? "ASC," : "DESC,"));
            }

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 字段筛选解析器
        /// </summary>
        /// <param name="lstExp">多个字段</param>
        /// <returns></returns>
        public string Select(List<Expression> lstExp)
        {
            Clear();
            if (lstExp == null || lstExp.Count == 0) { return null; }
            lstExp.ForEach(exp => _expNewProvider.Visit(exp, true));

            var sb = new StringBuilder();
            _expNewProvider.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 条件解析器
        /// </summary>
        /// <param name="exp">条件</param>
        /// <returns></returns>
        public string Where(Expression exp)
        {
            _expBoolProvider.Visit(exp);

            var sb = new StringBuilder();
            _expBoolProvider.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        private void Clear()
        {
            _expNewProvider.Clear();
            _expBoolProvider.Clear();
        }
    }
}