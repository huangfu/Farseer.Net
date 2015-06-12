using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class IEnumerableExtend
    {
        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst">要拼接的LIST</param>
        /// <param name="sign">分隔符</param>
        public static string ToString(this IEnumerable lst, string sign = ",")
        {
            if (lst == null) { return String.Empty; }

            var str = new StringBuilder();
            foreach (var item in lst) { str.Append(item + sign); }
            return str.ToString().DelEndOf(sign);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, int pageSize, int pageIndex = 1)
        {
            if (pageSize == 0) { return lst.ToList(); }

            #region 计算总页数

            var allCurrentPage = 0;
            var recordCount = lst.Count();
            if (pageIndex < 1) { pageIndex = 1; return lst.Take(pageSize).ToList(); }
            if (pageSize < 1) { pageSize = 10; }

            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage) { pageIndex = allCurrentPage; }

            #endregion

            return lst.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> IDs, int pageSize, int pageIndex = 1) where TEntity : IEntity
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID.GetValueOrDefault())), pageSize, pageIndex);
        }

        /// <summary>
        ///     复制一个新的List
        /// </summary>
        public static List<T> Copy<T>(this IEnumerable<T> lst)
        {
            var lstNew = new List<T>();
            if (lst == null) { return lstNew; }
            lstNew.AddRange(lst);
            return lstNew;
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TEntity ToNextInfo<TEntity>(this IEnumerable<TEntity> lst, int ID) where TEntity : IEntity
        {
            return lst.Where(o => o.ID > ID).OrderBy(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TEntity ToPreviousInfo<TEntity>(this IEnumerable<TEntity> lst, int ID) where TEntity : IEntity
        {
            return lst.Where(o => o.ID < ID).OrderByDescending(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取List列表
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> IDs) where TEntity : IEntity
        {
            return lst.Where(o => IDs.Contains(o.ID)).ToList();
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        public static TEntity ToEntity<TEntity>(this IEnumerable<TEntity> lst)
        {
            return lst.FirstOrDefault();
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID的操作</param>
        public static TEntity ToEntity<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.FirstOrDefault(o => o.ID == ID);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static int Count<TEntity>(this IEnumerable<TEntity> lst, List<int> IDs) where TEntity : IEntity
        {
            return lst.Count(o => IDs.Contains(o.ID));
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        public static int Count<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.Count(o => o.ID == ID);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst)
        {
            return lst.Any();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.Count(o => o.ID == ID) > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst, List<int> IDs) where TEntity : IEntity
        {
            return lst.Any(o => IDs.Contains(o.ID));
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">ModelInfo</typeparam>
        public static T GetValue<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select, T defValue = default(T))
        {
            if (lst == null) { return defValue; }
            var value = lst.Select(@select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">ModelInfo</typeparam>
        public static T GetValue<TEntity, T>(this IEnumerable<TEntity> lst, int? ID, Func<TEntity, T> select, T defValue = default(T)) where TEntity : IEntity
        {
            if (lst == null) { return defValue; }
            lst = lst.Where(o => o.ID == ID).ToList();
            if (!lst.Any())
            {
                return defValue;
            }

            var value = lst.Select(@select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select)
        {
            if (lst == null) { return null; }
            return lst.Select(@select).ToList();
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, int top, Func<TEntity, T> select)
        {
            return lst.Select(@select).Take(top).ToList();
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, List<int> IDs, Func<TEntity, T> select) where TEntity : IEntity
        {
            return lst.Where(o => IDs.Contains(o.ID)).ToSelectList(@select);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, List<int> IDs, int top, Func<TEntity, T> select) where TEntity : IEntity
        {
            return lst.Where(o => IDs.Contains(o.ID)).Take(top).ToSelectList(@select);
        }

        /// <summary>
        ///     克隆List
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable
        {
            return list == null ? null : list.Select(o => (T)o.Clone()).ToList();
        }

        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<uint> lst, uint? value)
        {
            return Enumerable.Contains(lst, value.GetValueOrDefault());
        }


        ///// <summary>
        ///// List转换成新的List
        ///// </summary>
        ///// <typeparam name="T1">源类型</typeparam>
        ///// <typeparam name="T2">新的类型</typeparam>
        ///// <param name="lst">源列表</param>
        ///// <param name="defValue">默认值</param>
        //public static List<T2> ToList<T1, T2>(this IEnumerable<T1> lst, T2 defValue) where T1 : struct
        //{
        //    List<T2> lstConvert = new List<T2>();
        //    foreach (var item in lst)
        //    {
        //        lstConvert.Add(item.ConvertType(defValue));
        //    }
        //    return lstConvert;
        //}

        ///// <summary>
        ///// List转换成新的List
        ///// </summary>
        ///// <typeparam name="T1">源类型</typeparam>
        ///// <typeparam name="T2">新的类型</typeparam>
        ///// <param name="lst">源列表</param>
        ///// <param name="func">转换方式</param>
        ///// <returns></returns>
        //public static List<T2> ToList<T1, T2>(this IEnumerable<T1> lst, Func<T1,T2> func) where T1 : struct
        //{
        //    List<T2> lstConvert = new List<T2>();
        //    foreach (var item in lst)
        //    {
        //        lstConvert.Add(func(item));
        //    }
        //    return lstConvert;
        //}



        ///// <summary>
        /////     不重复列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        ///// <param name="lst">列表</param>
        //public static List<TEntity> ToDistinctList<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select) where TEntity : class
        //{
        //    return lst.Distinct(new InfoComparer<TEntity, T>(select)).ToList();
        //}

        ///// <summary>
        /////     不重复列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        ///// <param name="lst">列表</param>
        //public static List<TEntity> ToDistinctList<TEntity, T>(this IEnumerable<TEntity> lst, List<int> IDs, Func<TEntity, T> select) where TEntity : class
        //{
        //    return lst.Where(o => IDs.Contains(o.ID)).Distinct(new InfoComparer<TEntity, T>(select)).ToList();
        //}
    }
}
