using System;
using System.Data;
using FS.Core;
using FS.Utils;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class DataRowExtend
    {
        /// <summary>
        ///     将DataRow转成实体类
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="dr">源DataRow</param>
        public static TEntity ToInfo<TEntity>(this DataRow dr) where TEntity : class,new()
        {
            var map = CacheManger.GetFieldMap(typeof(TEntity));
            var t = (TEntity)Activator.CreateInstance(typeof(TEntity));

            //赋值字段
            foreach (var kic in map.MapList)
            {
                if (dr.Table.Columns.Contains(kic.Value.FieldAtt.Name))
                {
                    if (!kic.Key.CanWrite) { continue; }
                    kic.Key.SetValue(t, dr[kic.Value.FieldAtt.Name].ConvertType(kic.Key.PropertyType), null);
                }
            }
            return t ?? new TEntity();
        }
    }
}
