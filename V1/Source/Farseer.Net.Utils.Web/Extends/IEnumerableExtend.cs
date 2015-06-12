using System.Collections.Generic;
using System.Linq;
using FS.Core.Infrastructure;
using FS.Utils.Web.UI.WebControls;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class IEnumerableExtend
    {
        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="rpt">Repeater</param>
        /// <returns></returns>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, Repeater rpt)
        {
            rpt.PageCount = lst.Count();
            return lst.ToList(rpt.PageSize, rpt.PageIndex);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="rpt">Repeater</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> IDs, Repeater rpt) where TEntity : IEntity
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID.GetValueOrDefault())), rpt);
        }
    }
}
