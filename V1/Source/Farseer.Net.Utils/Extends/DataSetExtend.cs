using System.Collections.Generic;
using System.Data;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class DataSetExtend
    {
        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="ds">源DataSet</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this DataSet ds) where T : class,new()
        {
            return ds.Tables.Count == 0 ? null : ds.Tables[0].ToList<T>();
        }
    }
}
