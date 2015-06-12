using System.Text;
using FS.Core.Data;
using FS.Core.Infrastructure;
using FS.Utils;

namespace FS.Core.Client.SqlServer
{
    /// <summary>
    /// 针对SqlServer 2000 数据库 提供
    /// </summary>
    public class SqlBuilder2000 : SqlBuilder
    {
        public SqlBuilder2000(BaseQueueManger queueManger, Queue queue) : base(queueManger, queue) { }

        public override Queue ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return Queue; }

            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;
            Queue.Sql = new StringBuilder();

            Check.IsTure(string.IsNullOrWhiteSpace(strOrderBySql) && Queue.FieldMap.PrimaryState.Key == null, "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", Queue.FieldMap.PrimaryState.Value.FieldAtt.Name) : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }

            Queue.Sql.AppendFormat("SELECT {0} TOP {2} {1} FROM (SELECT TOP {3} {1} FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, Queue.Name, strWhereSql, strOrderBySql, strOrderBySqlReverse);
            return Queue;
        }
    }
}