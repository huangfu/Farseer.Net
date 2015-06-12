using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FS.Core;
using FS.Core.Infrastructure;
using FS.Extends;

namespace FS.Utils
{
    /// <summary>
    /// 类型转换器
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue">要转换的源对象</param>
        /// <param name="defValue">转换失败时，代替的默认值</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static T ConvertType<T>(object sourceValue, T defValue = default(T))
        {
            if (sourceValue == null) { return defValue; }

            var returnType = typeof(T);
            var sourceType = sourceValue.GetType();

            // 相同类型，则直接返回原型
            if (Type.GetTypeCode(returnType) == Type.GetTypeCode(sourceType)) { return (T)sourceValue; }

            var val = ConvertType(sourceValue, returnType);
            return val != null ? (T)val : defValue;
        }

        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="defType">要转换的对象的类型</param>
        public static object ConvertType(object sourceValue, Type defType)
        {
            if (sourceValue == null) { return null; }

            // 对   Nullable<> 类型处理
            if (defType.IsGenericType && defType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return ConvertType(sourceValue, Nullable.GetUnderlyingType(defType)); }
            // 对   List 类型处理
            if (defType.IsGenericType && defType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                var objString = sourceValue.ToString();
                // List参数类型
                var argumType = defType.GetGenericArguments()[0];

                switch (Type.GetTypeCode(argumType))
                {
                    case TypeCode.Boolean: { return ToList(objString, false); }
                    case TypeCode.DateTime: { return ToList(objString, DateTime.MinValue); }
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single: { return ToList(objString, 0m); }
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16: { return ToList<ushort>(objString, 0); }
                    case TypeCode.UInt32: { return ToList<uint>(objString, 0); }
                    case TypeCode.UInt64: { return ToList<ulong>(objString, 0); }
                    case TypeCode.Int16: { return ToList<short>(objString, 0); }
                    case TypeCode.Int64: { return ToList<long>(objString, 0); }
                    case TypeCode.Int32: { return ToList(objString, 0); }
                    case TypeCode.Empty:
                    case TypeCode.Char:
                    case TypeCode.String: { return ToList(objString, ""); }
                }
            }

            return ConvertType(sourceValue, sourceValue.GetType(), defType);
        }

        /// <summary>
        /// 将值转换成类型对象的值（此方法作为公共的调用，只支持单值转换)
        /// </summary>
        /// <param name="objValue">要转换的值</param>
        /// <param name="objType">要转换的值类型</param>
        /// <param name="defType">转换失败时，代替的默认值类型</param>
        /// <returns></returns>
        public static object ConvertType(object objValue, Type objType, Type defType)
        {
            if (objValue == null) { return null; }
            if (objType == defType) { return objValue; }

            var objString = objValue.ToString();

            var defTypeCode = Type.GetTypeCode(defType);
            var objTypeCode = Type.GetTypeCode(objType);

            // 枚举处理
            if (defType.IsEnum)
            {
                if (string.IsNullOrWhiteSpace(objString)) { return null; }
                return IsType<int>(objString) ? Enum.ToObject(defType, int.Parse(objString)) : Enum.Parse(defType, objString, true);
            }
            // bool转int
            if (objTypeCode == TypeCode.Boolean)
            {
                switch (defTypeCode)
                {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64: return ConvertType(objValue, true) ? 1 : 0;
                }
            }

            switch (defTypeCode)
            {
                case TypeCode.Boolean: { return (object)(!string.IsNullOrWhiteSpace(objString) && (objString.Equals("on") || objString == "1" || objString.Equals("true"))); }
                case TypeCode.Byte: { Byte result; return Byte.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Char: { Char result; return Char.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.DateTime: { DateTime result; return DateTime.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Decimal: { Decimal result; return Decimal.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Double: { Double result; return Double.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Int16: { Int16 result; return Int16.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Int32: { Int32 result; return Int32.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Int64: { Int64 result; return Int64.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.SByte: { SByte result; return SByte.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Single: { Single result; return Single.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.UInt16: { UInt16 result; return UInt16.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.UInt32: { UInt32 result; return UInt32.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.UInt64: { UInt64 result; return UInt64.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Empty:
                case TypeCode.String: { return (object)objString; }
                case TypeCode.Object: { break; }
            }

            try { return Convert.ChangeType(objValue, defType); }
            catch { return null; }
        }

        /// <summary>
        ///     判断是否T类型
        /// </summary>
        /// <param name="sourceValue">要判断的对象</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static bool IsType<T>(object sourceValue)
        {
            if (sourceValue == null) { return false; }

            var defType = typeof(T);
            var objType = sourceValue.GetType();

            var defTypeCode = Type.GetTypeCode(defType);
            var objTypeCode = Type.GetTypeCode(objType);

            // 相同类型，则直接返回原型
            if (objTypeCode == defTypeCode) { return true; }

            // 判断是否为泛型
            if (defType.IsGenericType)
            {
                // 非 Nullable<> 类型
                if (defType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    return sourceValue is T;
                }
                // 对Nullable<>类型处理
                defType = Nullable.GetUnderlyingType(defType);
                defTypeCode = Type.GetTypeCode(defType);
            }

            if (defType.IsEnum) { return sourceValue is Enum; }
            var objString = sourceValue.ToString();

            switch (defTypeCode)
            {
                case TypeCode.Boolean: { return !string.IsNullOrWhiteSpace(objString) && (objString.Equals("on") || objString == "1" || objString.Equals("true")); }
                case TypeCode.Byte: { Byte result; return Byte.TryParse(objString, out result); }
                case TypeCode.Char: { Char result; return Char.TryParse(objString, out result); }
                case TypeCode.DateTime: { DateTime result; return DateTime.TryParse(objString, out result); }
                case TypeCode.Decimal: { Decimal result; return Decimal.TryParse(objString, out result); }
                case TypeCode.Double: { Double result; return Double.TryParse(objString, out result); }
                case TypeCode.Int16: { Int16 result; return Int16.TryParse(objString, out result); }
                case TypeCode.Int32: { Int32 result; return Int32.TryParse(objString, out result); }
                case TypeCode.Int64: { Int64 result; return Int64.TryParse(objString, out result); }
                case TypeCode.SByte: { SByte result; return SByte.TryParse(objString, out result); }
                case TypeCode.Single: { Single result; return Single.TryParse(objString, out result); }
                case TypeCode.UInt16: { UInt16 result; return UInt16.TryParse(objString, out result); }
                case TypeCode.UInt32: { UInt32 result; return UInt32.TryParse(objString, out result); }
                case TypeCode.UInt64: { UInt64 result; return UInt64.TryParse(objString, out result); }
                case TypeCode.Empty:
                case TypeCode.String: { return true; }
                case TypeCode.Object: { break; }
            }
            return objType == defType;
        }

        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="splitString">分隔符为NullOrEmpty时，则直接拆份为Char</param>
        /// <param name="defValue">默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值)</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static List<T> ToList<T>(string str, T defValue, string splitString = ",")
        {
            var lst = new List<T>();
            if (string.IsNullOrWhiteSpace(str)) { return lst; }

            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (string.IsNullOrWhiteSpace(splitString))
            {
                for (var i = 0; i < str.Length; i++) { lst.Add(ConvertType(str.Substring(i, 1), defValue)); }
            }
            else
            {
                var strArray = splitString.Length == 1 ? str.Split(splitString[0]) : str.Split(new string[1] { splitString }, StringSplitOptions.None);
                lst.AddRange(strArray.Select(item => ConvertType(item, defValue)));
            }
            return lst;
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="lst">集合</param>
        /// <returns></returns>
        public static DataTable ToTable<TEntity>(List<TEntity> lst) where TEntity : class
        {
            var dt = new DataTable();
            if (lst.Count == 0) { return dt; }
            var map = CacheManger.GetFieldMap(lst[0].GetType());
            var lstFields = map.MapList.Where(o => o.Value.FieldAtt.IsMap).ToList();
            foreach (var field in lstFields)
            {
                var type = field.Key.PropertyType;
                // 对   List 类型处理
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
                dt.Columns.Add(field.Value.FieldAtt.Name, type);
            }

            foreach (var info in lst)
            {
                dt.Rows.Add(dt.NewRow());
                foreach (var field in lstFields)
                {
                    var value = GetValue(info, field.Key.Name, (object)null);
                    if (value == null) { continue; }
                    if (!dt.Columns.Contains(field.Value.FieldAtt.Name)) { dt.Columns.Add(field.Value.FieldAtt.Name); }
                    dt.Rows[dt.Rows.Count - 1][field.Value.FieldAtt.Name] = value;
                }
            }
            return dt;
        }

        /// <summary>
        ///     DataTable转换为实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(DataTable dt) where TEntity : class, new()
        {
            var list = new List<TEntity>();
            var map = CacheManger.GetFieldMap(typeof(TEntity));
            foreach (DataRow dr in dt.Rows)
            {
                // 赋值字段
                var t = new TEntity();
                foreach (var kic in map.MapList)
                {
                    if (!kic.Key.CanWrite) { continue; }
                    var filedName = !DbProvider.IsField(kic.Value.FieldAtt.Name) ? kic.Key.Name : kic.Value.FieldAtt.Name;
                    if (dr.Table.Columns.Contains(filedName))
                    {
                        kic.Key.SetValue(t, ConvertType(dr[filedName], kic.Key.PropertyType), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        ///     查找对象属性值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static T GetValue<TEntity, T>(TEntity info, string propertyName, T defValue = default(T)) where TEntity : class
        {
            if (info == null) { return defValue; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanRead) { return defValue; }
                return ConvertType(property.GetValue(info, null), defValue);
            }
            return defValue;
        }

        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(IDataReader reader) where TEntity : class, new()
        {
            var list = new List<TEntity>();
            var map = CacheManger.GetFieldMap(typeof(TEntity));

            while (reader.Read())
            {
                var t = (TEntity)Activator.CreateInstance(typeof(TEntity));

                //赋值字段
                foreach (var kic in map.MapList)
                {
                    if (HaveName(reader, kic.Value.FieldAtt.Name))
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(t, reader[kic.Value.FieldAtt.Name].ConvertType(kic.Key.PropertyType), null);
                    }
                }

                list.Add(t);
            }
            reader.Close();
            return list;
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToInfo<TEntity>(IDataReader reader) where TEntity : class, new()
        {
            var map = CacheManger.GetFieldMap(typeof(TEntity));

            var t = (TEntity)Activator.CreateInstance(typeof(TEntity));
            var isHaveValue = false;

            if (reader.Read())
            {
                //赋值字段
                foreach (var kic in map.MapList)
                {
                    if (HaveName(reader, kic.Value.FieldAtt.Name))
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(t, reader[kic.Value.FieldAtt.Name].ConvertType(kic.Key.PropertyType), null);
                        isHaveValue = true;
                    }
                }
            }
            reader.Close();
            return isHaveValue ? t : null;
        }

        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HaveName(IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).IsEquals(name)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// 动态创建一个 o.ID == value的表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="val">右值</param>
        /// <param name="expType">匹配符号类型</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<TEntity, bool>> CreateBinaryExpression<TEntity>(object val, ExpressionType expType = ExpressionType.Equal, string memberName = "ID") where TEntity : class, new()
        {
            var oParam = Expression.Parameter(typeof(TEntity), "o");
            var left = Expression.MakeMemberAccess(oParam, typeof(TEntity).GetMember(memberName)[0]);
            var right = Expression.Constant(val, left.Type);
            BinaryExpression where = null;
            switch (expType)
            {
                case ExpressionType.Equal: where = Expression.Equal(left, right); break;
                case ExpressionType.NotEqual: where = Expression.NotEqual(left, right); break;
                case ExpressionType.GreaterThan: where = Expression.GreaterThan(left, right); break;
                case ExpressionType.GreaterThanOrEqual: where = Expression.GreaterThanOrEqual(left, right); break;
                case ExpressionType.LessThan: where = Expression.LessThan(left, right); break;
                case ExpressionType.LessThanOrEqual: where = Expression.LessThanOrEqual(left, right); break;
            }
            return (Expression<Func<TEntity, bool>>)Expression.Lambda(where, oParam);
        }

        /// <summary>
        /// 动态创建一个 List.Contains(o.ID)的表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="val">右值</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<TEntity, bool>> CreateContainsBinaryExpression<TEntity>(object val, string memberName = "ID") where TEntity : class, new()
        {
            var oParam = Expression.Parameter(typeof(TEntity), "o");
            var left = Expression.MakeMemberAccess(oParam, typeof(TEntity).GetMember(memberName)[0]);
            var leftx = Expression.Call(left, left.Type.GetMethod("GetValueOrDefault", new Type[] { }));

            var right = Expression.Constant(val, val.GetType());
            var where = Expression.Call(right, right.Type.GetMethod("Contains"), leftx);
            return (Expression<Func<TEntity, bool>>)Expression.Lambda(where, oParam);
        }
    }
}