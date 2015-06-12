using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class TypeExtend
    {
        /// <summary>
        ///     枚举转ListItem
        /// </summary>
        public static List<ListItem> ToListItem(this Type enumType)
        {
            return (from int value in Enum.GetValues(enumType) select new ListItem(((Enum)Enum.ToObject(enumType, value)).GetName(), value.ToString(CultureInfo.InvariantCulture))).ToList();
        }
    }
}
