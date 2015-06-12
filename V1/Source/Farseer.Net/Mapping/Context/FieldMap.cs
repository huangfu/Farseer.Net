using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using FS.Core;
using FS.Mapping.Context.Attribute;

namespace FS.Mapping.Context
{
    /// <summary>
    ///     字段 映射关系
    /// </summary>
    public class FieldMap
    {
        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, FieldState> MapList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public FieldMap(Type type)
        {
            Type = type;
            MapList = new Dictionary<PropertyInfo, FieldState>();

            #region 变量属性

            // 循环Set的字段
            foreach (var fieldProperty in Type.GetProperties())
            {
                // 获取字段的特性
                var attField = fieldProperty.GetCustomAttributes(false);
                var fieldState = new FieldState();
                foreach (var attr in attField)
                {
                    // 数据类型
                    if (attr is DataTypeAttribute) { fieldState.DataType = (DataTypeAttribute)attr; continue; }
                    // 字段映射
                    if (attr is FieldAttribute) { fieldState.FieldAtt = (FieldAttribute)attr; continue; }
                    // 属性扩展
                    if (attr is PropertyExtendAttribute) { fieldState.PropertyExtend = ((PropertyExtendAttribute)attr).PropertyExtend; continue; }
                }

                if (fieldState.FieldAtt == null) { fieldState.FieldAtt = new FieldAttribute { Name = fieldProperty.Name }; }
                if (string.IsNullOrEmpty(fieldState.FieldAtt.Name)) { fieldState.FieldAtt.Name = fieldProperty.Name; }
                if (fieldState.FieldAtt.IsMap && fieldState.FieldAtt.IsPrimaryKey) { PrimaryState = new KeyValuePair<PropertyInfo, FieldState>(fieldProperty, fieldState); } else { fieldState.FieldAtt.IsPrimaryKey = false; }


                //添加属变量标记名称
                MapList.Add(fieldProperty, fieldState);
            }

            #endregion
        }

        public KeyValuePair<PropertyInfo, FieldState> PrimaryState { get; private set; }

        /// <summary>
        ///     类型
        /// </summary>
        private Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator FieldMap(Type type)
        {
            return CacheManger.GetFieldMap(type);
        }

        /// <summary>
        ///     获取当前属性（通过使用的fieldName）
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        public KeyValuePair<PropertyInfo, FieldState> GetState(string fieldName)
        {
            return string.IsNullOrEmpty(fieldName) ? MapList.FirstOrDefault(o => o.Value.FieldAtt.IsPrimaryKey) : MapList.FirstOrDefault(o => o.Key.Name == fieldName);
        }
    }
}