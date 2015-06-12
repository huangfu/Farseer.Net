using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using FS.Core.Infrastructure;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class WebDropDownListExtend
    {
        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="parentID">所属上级节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TEntity>(this List<TEntity> lstCate, DropDownList ddl, int selectedValue, int parentID, bool isUsePrefix = true) where TEntity : ICate, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, parentID, 0, null, false, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="where">筛选条件</param>
        /// <param name="isContainsSub">筛选条件是否包含子节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TEntity>(this List<TEntity> lstCate, DropDownList ddl, int selectedValue = 0, Func<TEntity, bool> where = null, bool isContainsSub = false, bool isUsePrefix = true) where TEntity : ICate, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, 0, 0, where, isContainsSub, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     递归绑定
        /// </summary>
        private static void Bind<TEntity>(this List<TEntity> lstCate, DropDownList ddl, int parentID, int tagNum, Func<TEntity, bool> where, bool isContainsSub, bool isUsePrefix) where TEntity : ICate, new()
        {
            List<TEntity> lst;

            lst = lstCate.FindAll(o => o.ParentID == parentID);
            if (lst == null || lst.Count == 0) { return; }

            if ((parentID == 0 || isContainsSub) && where != null) { lst = lst.Where(where).ToList(); }
            if (lst == null || lst.Count == 0) { return; }

            foreach (var info in lst)
            {
                var text = isUsePrefix ? new string('　', tagNum) + "├─" + info.Caption : info.Caption;

                ddl.Items.Add(new ListItem { Value = info.ID.ToString(), Text = text });
                lstCate.Bind(ddl, info.ID.Value, tagNum + 1, where, isContainsSub, isUsePrefix);
            }
        }
    }
}
