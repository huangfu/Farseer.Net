using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class TextBoxExtend
    {
        /// <summary>
        ///     清除空格
        /// </summary>
        /// <param name="control">TextBox控件</param>
        public static string Trim(this TextBox control)
        {
            control.Text = control.Text.Trim();
            return control.Text;
        }
        /// <summary>
        ///     WinForm绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="lst">List列表</param>
        /// <param name="dataTextField">显示名称</param>
        /// <param name="dataValueField">值</param>
        public static void Bind(this ListControl control, IEnumerable lst, string dataTextField = "Caption", string dataValueField = "ID")
        {
            control.DisplayMember = dataTextField;
            control.ValueMember = dataValueField;

            control.DataSource = lst;
        }

        /// <summary>
        ///     WinForm绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="eumType">枚举类型</param>
        public static void Bind(this ListControl control, Type eumType)
        {
            var lst = new List<string>();
            foreach (var item in eumType.ToDictionary()) { lst.Add(item.Value); }

            control.DataSource = lst;
        }

        /// <summary>
        ///     WinForm绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="eumType">枚举类型</param>
        public static void Bind(this DataGridViewComboBoxColumn control, Type eumType)
        {
            control.DataSource = eumType.ToDictionary();
            control.ValueMember = "Value";
            control.DisplayMember = "Text";
        }
        ///// <summary>
        /////     绑定到DropDownList
        ///// </summary>
        ///// <param name="ddl">要绑定的ddl控件</param>
        ///// <param name="lstInfo">要进行绑定的列表</param>
        ///// <param name="selectedValue">默认选则值</param>
        ///// <param name="RemoveID">不加载的节点（包括子节点）</param>
        //public static void Bind(this DropDownList ddl, List<ModelCateInfo> lstInfo, object selectedValue = null, int RemoveID = -1)
        //{
        //    ddl.Items.Clear();

        //    Bind(ddl, lstInfo, 0, 0, RemoveID);

        //    if (selectedValue != null)
        //    {
        //        ddl.SelectedItems(selectedValue);
        //    }
        //}

        ///// <summary>
        /////     递归绑定
        ///// </summary>
        //private static void Bind(DropDownList ddl, List<ModelCateInfo> lstInfo, int parentID = 0, int tagNum = 0, int RemoveID = 1)
        //{
        //    if (lstInfo == null || lstInfo.Count == 0)
        //    {
        //        return;
        //    }

        //    var lstModelCateInfo = lstInfo.FindAll(o => o.ParentID == parentID);

        //    if (lstInfo == null || lstInfo.Count == 0)
        //    {
        //        return;
        //    }

        //    foreach (var info in lstModelCateInfo)
        //    {
        //        if (info.ID == RemoveID)
        //        {
        //            continue;
        //        }
        //        ddl.Items.Add(new ListItem
        //        {
        //            Value = info.ID.ToString(),
        //            Text = new string('　', tagNum) + "├─" + info.Caption
        //        });
        //        Bind(ddl, lstInfo, info.ID.Value, tagNum + 1, RemoveID);
        //    }
        //}

        /// <summary>
        ///     IEnumerable绑定到DataGridView
        /// </summary>
        /// <param name="dgv">DataGridView</param>
        /// <param name="lst">List列表</param>
        public static void Bind<T>(this DataGridView dgv, List<T> lst)
        {
            var bind = new BindingList<T>(lst);
            dgv.DataSource = bind;
        }

        /// <summary>
        ///     IEnumerable绑定到DataGridView
        /// </summary>
        /// <param name="dgv">DataGridView</param>
        /// <param name="lst">List列表</param>
        public static void Bind<T>(this DataGridView dgv, BindingList<T> lst, Action<object, ListChangedEventArgs> act = null)
        {
            if (act != null) { lst.ListChanged += (o, e) => { act(o, e); }; }
            dgv.DataSource = lst;
        }
    }
}
