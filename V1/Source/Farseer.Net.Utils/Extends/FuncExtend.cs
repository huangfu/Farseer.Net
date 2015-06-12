using System;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class FuncExtend
    {
        /// <summary>
        ///     Func 转换成 Predicate 对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="source">源Func对象</param>
        public static Predicate<T> ToPredicate<T>(this Func<T, bool> source) where T : class
        {
            return new Predicate<T>(source);
        }
    }
}
