using System.Web.UI;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     实体类扩展方法
    /// </summary>
    public static class ModelExtend
    {
        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="info">要填入数据的实体类</param>
        /// <param name="contentPlaceHolderID">母版页面版ID</param>
        /// <param name="prefix">控件前缀</param>
        public static void Fill<TEntity>(this TEntity info, System.Web.UI.Page page, string contentPlaceHolderID, string prefix = "hl") where TEntity : class
        {
            if (info == null)
            {
                return;
            }

            var masterControl = page.FindControl(contentPlaceHolderID);
            if (masterControl == null)
            {
                return;
            }

            Fill(masterControl.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="info">要填充的值</param>
        public static void Fill<TEntity>(this TEntity info, System.Web.UI.Page page, string prefix = "hl") where TEntity : class
        {
            if (info == null) { return; }
            Fill(page.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="controls">页面控件集合</param>
        /// <param name="infoValue">所属实体类的值</param>
        /// <param name="prefix">前缀</param>
        private static void Fill<TEntity>(ControlCollection controls, TEntity infoValue, string prefix = "hl") where TEntity : class
        {
            //if (infoValue == null || controls == null)
            //{
            //    return;
            //}
            //var map = TableMapCache.GetMap(infoValue);
            //foreach (var kic in map.ModelList)
            //{
            //    // 当前成员的值
            //    var value = kic.Key.GetValue(infoValue, null);
            //    if (value == null) { continue; }

            //    var type = value.GetType();

            //    // 当前成员，是一个类
            //    if (value is ModelInfo)
            //    {
            //        foreach (var item in type.GetProperties()) { Fill(controls, (ModelInfo)value, prefix); }
            //        continue;
            //    }

            //    foreach (Control item in controls)
            //    {
            //        var control = item.FindControl(prefix + kic.Key.Name);
            //        if (control == null) { continue; }

            //        if (control is HiddenField)
            //        {
            //            ((HiddenField)control).Value = value.ToString();
            //            break;
            //        }
            //        if (control is CheckBox) { ((CheckBox)control).Checked = value.ConvertType(false); break; }
            //        if (control is CheckBoxList)
            //        {
            //            // 数据保存的是数字以逗号分隔的数据，并且是ListControl的控件，则可以直接填充数据
            //            if (value is string)
            //            {
            //                var lstIDs = value.ToString().ToList(0);
            //                ((CheckBoxList)control).SetValue(lstIDs);
            //                break;
            //            }

            //            // 枚举为二进制时
            //            var types = kic.Key.PropertyType.GetGenericArguments();
            //            if (types != null && types.Length > 0 && types[0].IsEnum)
            //            {
            //                var att = types[0].GetCustomAttributes(typeof(FlagsAttribute), false);

            //                if (att != null && att.Length > 0)
            //                {
            //                    foreach (ListItem listItem in ((CheckBoxList)control).Items)
            //                    {
            //                        var itemValue = listItem.Value.ConvertType(0);
            //                        listItem.Selected = (value.ConvertType(0) & itemValue) == itemValue;
            //                    }
            //                    break;
            //                }
            //            }

            //        }
            //        if (control is ListControl)
            //        {
            //            ((ListControl)control).SelectedItems(value);
            //            break;

            //        }

            //        if (value is Enum) { value = ((Enum)value).GetName(); }
            //        if (value is IList) { value = ((IList)value).ToString(","); }
            //        if (value is bool) { value = ((bool)value) ? "是" : "否"; }

            //        if (control is TextBox) { ((TextBox)control).Text = value.ToString(); break; }
            //        if (control is Label) { ((Label)control).Text = value.ToString(); break; }
            //        if (control is Literal) { ((Literal)control).Text = value.ToString(); break; }
            //        if (control is Image) { ((Image)control).ImageUrl = value.ToString(); break; }
            //        if (control is HyperLink) { ((HyperLink)control).NavigateUrl = value.ToString(); break; }
            //    }
            //}
        }

    }
}