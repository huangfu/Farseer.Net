using System.Collections.Generic;
using FS.Core.Data.Table;
using FS.Core.Infrastructure;
using FS.Utils.Web.UI.WebControls;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class SetExtend
    {
        /// <summary>
        ///     通用的分页方法(多条件)
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <param name="rpt">Repeater带分页控件</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this TableSet<TEntity> ts, Repeater rpt) where TEntity : class, IEntity, new()
        {
            int recordCount;
            var lst = ts.ToList(rpt.PageSize, rpt.PageIndex, out recordCount);
            rpt.PageCount = recordCount;

            return lst;
        }
    }
}
