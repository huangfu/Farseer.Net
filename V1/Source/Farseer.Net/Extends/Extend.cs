using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FS.Core;
using FS.Utils;

namespace FS.Extends
{
    public static class Extend
    {
        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue">要转换的源对象</param>
        /// <param name="defValue">转换失败时，代替的默认值</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static T ConvertType<T>(this object sourceValue, T defValue = default(T))
        {
            return ConvertHelper.ConvertType(sourceValue, defValue);
        }

        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="defType">要转换的对象的类型</param>
        public static object ConvertType(this object sourceValue, Type defType)
        {
            return ConvertHelper.ConvertType(sourceValue, defType);
        }

        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="splitString">分隔符为NullOrEmpty时，则直接拆份为Char</param>
        /// <param name="defValue">默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值)</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static List<T> ToList<T>(this string str, T defValue, string splitString = ",")
        {
            return ConvertHelper.ToList(str, defValue, splitString);
        }

        /// <summary>
        ///     DataTable转换为实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this DataTable dt) where TEntity : class, new()
        {
            return ConvertHelper.ToList<TEntity>(dt);
        }
        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this IDataReader reader) where TEntity : class, new()
        {
            return ConvertHelper.ToList<TEntity>(reader);
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToEntity<TEntity>(this IDataReader reader) where TEntity : class, new()
        {
            return ConvertHelper.ToInfo<TEntity>(reader);
        }

        /// <summary>
        ///     And 操作
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(this Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            if (left == null) { return right; }
            if (right == null) { return left; }

            var leftParam = left.Parameters[0];
            var rightParam = right.Parameters[0];
            return Expression.Lambda<Func<TEntity, bool>>(ReferenceEquals(leftParam, rightParam) ? Expression.AndAlso(left.Body, right.Body) : Expression.AndAlso(left.Body, Expression.Invoke(right, leftParam)), leftParam);
        }

        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<int> lst, int? value)
        {
            return Enumerable.Contains(lst, value.GetValueOrDefault());
        }

        /// <summary>
        ///     比较两者是否相等，不考虑大小写,两边空格
        /// </summary>
        /// <param name="str">对比一</param>
        /// <param name="str2">对比二</param>
        /// <returns></returns>
        public static bool IsEquals(this string str, string str2)
        {
            if (str == str2)
            {
                return true;
            }
            if (str == null || str2 == null)
            {
                return false;
            }
            if (str.Trim().Length != str2.Trim().Length)
            {
                return false;
            }
            return String.Compare(str.Trim(), str2.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        ///     Func 转换成 Predicate 对象
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="source">源Func对象</param>
        public static Predicate<TEntity> ToPredicate<TEntity>(this Func<TEntity, bool> source) where TEntity : class, new()
        {
            return new Predicate<TEntity>(source);
        }

        /// <summary>
        ///     获取字段名称
        /// </summary>
        /// <param name="select">字段名称</param>
        /// <returns></returns>
        public static string GetUsedName<T1, T2>(this Expression<Func<T1, T2>> select) where T1 : class
        {
            MemberExpression memberExpression;

            var unary = @select.Body as UnaryExpression;
            if (unary != null)
            {
                memberExpression = unary.Operand as MemberExpression;
            }
            else if (@select.Body.NodeType == ExpressionType.Call)
            {
                memberExpression = (MemberExpression)((MethodCallExpression)@select.Body).Object;
            }
            else
            {
                memberExpression = @select.Body as MemberExpression;
            }

            var map = CacheManger.GetFieldMap(typeof(T1));
            var modelInfo = map.GetState((memberExpression.Member).Name);

            return modelInfo.Value.FieldAtt.Name;
        }

        /// <summary>
        ///     获取字段名称
        /// </summary>
        /// <param name="select">字段名称</param>
        /// <returns></returns>
        public static List<string> GetMemberName(this Expression select)
        {
            switch (select.NodeType)
            {
                case ExpressionType.New:
                    {
                        var lst = new List<string>();
                        foreach (var item in ((NewExpression)select).Arguments) { lst.AddRange(GetMemberName(item)); }
                        return lst;
                    }
                case ExpressionType.Lambda: return GetMemberName(((LambdaExpression)select).Body);
                case ExpressionType.MemberAccess:
                    {
                        var m = ((MemberExpression)select);
                        return new List<string> { m.Member.Name };
                    };
            };
            return new List<string>();
        }
    }
}