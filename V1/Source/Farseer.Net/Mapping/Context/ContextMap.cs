using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.Core;
using FS.Mapping.Context.Attribute;

namespace FS.Mapping.Context
{
    /// <summary>
    ///     TableContext、ProcContext、ViewContext 映射关系
    /// </summary>
    public class ContextMap
    {
        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, SetState> MapList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public ContextMap(Type type)
        {
            Type = type;
            MapList = new Dictionary<PropertyInfo, SetState>();

            #region 类属性
            var attContext = Type.GetCustomAttributes(typeof(ContextAttribute), false);
            foreach (var attr in attContext.OfType<ContextAttribute>()) { ContextProperty = attr; }
            if (ContextProperty == null) { ContextProperty = new ContextAttribute(); }
            #endregion

            #region 变量属性

            // 遍历所有Set属性(表、视图、存储过程的名称),取得对应使用标记名称
            foreach (var setProperty in Type.GetProperties())
            {
                if (!new List<string> { "ViewSet`1", "ViewSetCache`1", "TableSet`1", "TableSetCache`1", "ProcSet`1" }.Contains(setProperty.PropertyType.Name)) { continue; }
                var setState = new SetState();

                #region Set属性
                var setAtt = setProperty.GetCustomAttributes(typeof(SetAttribute), false);
                foreach (var attr in setAtt.OfType<SetAttribute>()) { setState.SetAtt = attr; }
                if (setState.SetAtt == null) { setState.SetAtt = new SetAttribute(); }
                if (string.IsNullOrWhiteSpace(setState.SetAtt.Name)) { setState.SetAtt.Name = setProperty.Name; }
                #endregion

                // 映射Set属性
                setState.FieldMap = CacheManger.GetFieldMap(setProperty.PropertyType.GetGenericArguments()[0]);

                //添加属变量标记名称
                MapList.Add(setProperty, setState);
            }
            #endregion
        }

        /// <summary>
        ///     类属性
        /// </summary>
        public ContextAttribute ContextProperty { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        private Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator ContextMap(Type type)
        {
            return CacheManger.GetContextMap(type);
        }

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="setPropertyInfo">属性变量</param>
        /// <returns></returns>
        public KeyValuePair<PropertyInfo, SetState> GetState(PropertyInfo setPropertyInfo)
        {
            return MapList.Single(o => o.Key == setPropertyInfo);
        }

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="setType">属性变量</param>
        /// <param name="propertyName">属性名称</param>
        public KeyValuePair<PropertyInfo, SetState> GetState(Type setType, string propertyName)
        {
            return MapList.FirstOrDefault(o => o.Key.PropertyType == setType && o.Key.Name == propertyName);
        }
    }
}