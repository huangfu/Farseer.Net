using System.Text;
using FS.Core.Data;
using FS.Core.Infrastructure;
using FS.Utils;

namespace FS.Core.Client.Common
{
    public class SqlBuilder : ISqlBuilder
    {
        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly BaseQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly Queue Queue;
        /// <summary>
        /// 数据库字段解析器总入口，根据要解析的类型，再分散到各自负责的解析器
        /// </summary>
        protected readonly ExpressionVisit Visit;

        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public SqlBuilder(BaseQueueManger queueManger, Queue queue)
        {
            QueueManger = queueManger;
            Queue = queue;
            Visit = new ExpressionVisit(queueManger, Queue);
        }

        public virtual Queue ToEntity()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
            return Queue;
        }

        public virtual Queue ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strTopSql = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand)
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} {5}", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2}{5} FROM {3} {4} ORDER BY NEWID()", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, isDistinct ? ",NEWID() as newid" : "");
            }
            else
            {
                Queue.Sql.AppendFormat("SELECT {2} FROM (SELECT {0} {1} *{6} FROM {3} {4} ORDER BY NEWID()) a {5}", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql, isDistinct ? ",NEWID() as newid" : "");
            }
            return Queue;
        }

        public virtual Queue ToList(int pageSize, int pageIndex, bool isDistinct = false)
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

            Queue.Sql.AppendFormat("SELECT {0} TOP {2} {1} FROM (SELECT TOP {3} * FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, Queue.Name, strWhereSql, strOrderBySql, strOrderBySqlReverse);
            return Queue;
        }

        public virtual Queue Count(bool isDistinct = false)
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT {0} Count(0) FROM {1} {2}", strDistinctSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
            return Queue;
        }

        public virtual Queue Sum()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT SUM({0}) FROM {1} {2}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
            return Queue;
        }

        public virtual Queue Max()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT MAX({0}) FROM {1} {2}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
            return Queue;
        }

        public virtual Queue Min()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT MIN({0}) FROM {1} {2}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
            return Queue;
        }

        public virtual Queue GetValue()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
            return Queue;
        }

        public virtual Queue Delete()
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("DELETE FROM {0} {1}", QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
            return Queue;
        }

        public virtual Queue Insert<TEntity>(TEntity entity) where TEntity : class,new()
        {
            Queue.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            Queue.Sql.AppendFormat("INSERT INTO {0} {1}", Queue.Name, strinsertAssemble);
            return Queue;
        }

        public virtual Queue InsertIdentity<TEntity>(TEntity entity) where TEntity : class,new()
        {
            Queue.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);
            Queue.Sql.AppendFormat("INSERT INTO {0} {1}", Queue.Name, strinsertAssemble);
            return Queue;
        }

        public virtual Queue Update<TEntity>(TEntity entity) where TEntity : class,new()
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strAssemble = Visit.Assign(entity);
            var readCondition = Visit.ReadCondition(entity);

            Check.NotEmpty(strAssemble, "更新操作时，当前实体没有要更新的字段。" + typeof(TEntity));

            // 主键如果有值、或者设置成只读条件，则自动转成条件
            if (!string.IsNullOrWhiteSpace(readCondition)) { strWhereSql += string.IsNullOrWhiteSpace(strWhereSql) ? readCondition : " AND " + readCondition; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("UPDATE {0} SET {1} {2}", QueueManger.DbProvider.KeywordAegis(Queue.Name), strAssemble, strWhereSql);
            return Queue;
        }

        public virtual Queue AddUp()
        {
            Check.IsTure(Queue.ExpAssign == null || Queue.ExpAssign.Count == 0, "赋值的参数不能为空！");

            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            #region 字段赋值
            var sqlAssign = new StringBuilder();
            foreach (var keyValue in Queue.ExpAssign)
            {
                var strAssemble = Visit.Assign(keyValue.Key);
                var strs = strAssemble.Split(',');
                foreach (var s in strs) { sqlAssign.AppendFormat("{0} = {0} + {1},", s, keyValue.Value); }
            }
            if (sqlAssign.Length > 0) { sqlAssign = sqlAssign.Remove(sqlAssign.Length - 1, 1); }
            #endregion

            Queue.Sql.AppendFormat("UPDATE {0} SET {1} {2}", QueueManger.DbProvider.KeywordAegis(Queue.Name), sqlAssign, strWhereSql);
            return Queue;
        }
    }
}
