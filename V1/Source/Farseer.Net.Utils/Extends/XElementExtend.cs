using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FS.Core;
using FS.Mapping.Context;
using FS.Utils;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    /// XML扩展
    /// </summary>
    public static class XElementExtend
    {
        /// <summary>
        ///     将XML转成实体
        /// </summary>
        public static List<TEntity> ToList<TEntity>(this XElement element) where TEntity : class
        {
            var orm = CacheManger.GetFieldMap(typeof(TEntity));
            var list = new List<TEntity>();

            foreach (var el in element.Elements())
            {
                var t = (TEntity)Activator.CreateInstance(typeof(TEntity));

                //赋值字段
                foreach (var kic in orm.MapList)
                {
                    var type = kic.Key.PropertyType;
                    if (!kic.Key.CanWrite) { continue; }
                    switch (kic.Value.PropertyExtend)
                    {
                        case eumPropertyExtend.Attribute:
                            if (el.Attribute(kic.Value.FieldAtt.Name) == null) { continue; }
                            kic.Key.SetValue(t, el.Attribute(kic.Value.FieldAtt.Name).Value.ConvertType(type), null);
                            break;
                        case eumPropertyExtend.Element:
                            if (el.Element(kic.Value.FieldAtt.Name) == null) { continue; }
                            kic.Key.SetValue(t, el.Element(kic.Value.FieldAtt.Name).Value.ConvertType(type), null);
                            break;
                    }
                }
                list.Add(t);
            }
            return list;
        }
    }
}
