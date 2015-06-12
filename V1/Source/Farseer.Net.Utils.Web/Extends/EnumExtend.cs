using System;
using System.Collections.Generic;
using System.Web.Mvc;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static class WebEnumExtend
    {
        /// <summary>
        ///     枚举转ListItem
        /// </summary>
        public static List<SelectListItem> ToSelectListItem(this Type enumType)
        {
            var lst = new List<SelectListItem>();
            foreach (int value in Enum.GetValues(enumType))
            {
                lst.Add(new SelectListItem
                {
                    Value = value.ToString(),
                    Text = ((Enum)Enum.ToObject(enumType, value)).GetName()
                });
            }
            return lst;
        }
    }
}