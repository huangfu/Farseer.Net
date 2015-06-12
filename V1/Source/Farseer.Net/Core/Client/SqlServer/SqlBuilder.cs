using System;
using System.Text;
using FS.Core.Data;
using FS.Core.Infrastructure;
using FS.Utils;

namespace FS.Core.Client.SqlServer
{
    public class SqlBuilder : Common.SqlBuilder
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public SqlBuilder(BaseQueueManger queueManger, Queue queue) : base(queueManger, queue) { }

        public override Queue ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return Queue; }

            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            Queue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Check.IsTure(string.IsNullOrWhiteSpace(strOrderBySql) && (Queue.FieldMap.PrimaryState.Value == null || string.IsNullOrWhiteSpace(Queue.FieldMap.PrimaryState.Value.FieldAtt.Name)), "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", Queue.FieldMap.PrimaryState.Value.FieldAtt.Name) : strOrderBySql);

            Queue.Sql.AppendFormat("SELECT {1} FROM (SELECT {0} {1},ROW_NUMBER() OVER({2}) as Row FROM {3} {4}) a WHERE Row BETWEEN {5} AND {6};", strDistinctSql, strSelectSql, strOrderBySql, Queue.Name, strWhereSql, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
            return Queue;
        }

        public override Queue Insert<TEntity>(TEntity entity)
        {
            base.Insert(entity);

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            var indexHaveValue = Queue.FieldMap.PrimaryState.Key != null && Queue.FieldMap.PrimaryState.Key.GetValue(entity, null) != null;
            if (indexHaveValue && !string.IsNullOrWhiteSpace(Queue.FieldMap.PrimaryState.Value.FieldAtt.Name))
            {
                Queue.Sql = new StringBuilder(string.Format("SET IDENTITY_INSERT {0} ON ; {1} ; SET IDENTITY_INSERT {0} OFF;", Queue.Name, Queue.Sql));
            }
            return Queue;
        }

        public override Queue InsertIdentity<TEntity>(TEntity entity)
        {
            Insert(entity);
            Queue.Sql.AppendFormat("SELECT @@IDENTITY;");
            return Queue;
        }
    }
}