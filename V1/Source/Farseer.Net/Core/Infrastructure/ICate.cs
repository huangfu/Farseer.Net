﻿namespace FS.Core.Infrastructure
{
    public interface ICate<T> : IEntity<T>
    {
        /// <summary>
        ///     所属ID
        /// </summary>
        T ParentID { get; set; }
        /// <summary>
        ///     标题
        /// </summary>
        string Caption { get; set; }
        /// <summary>
        ///     排序
        /// </summary>
        int? Sort { get; set; }
    }
    public interface ICate : ICate<int?> { }
}
