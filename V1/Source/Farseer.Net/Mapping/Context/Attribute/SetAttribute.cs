using System;

namespace FS.Mapping.Context.Attribute
{
    /// <summary>
    /// 设置表、视图、存储过程的名称在数据库中的映射关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SetAttribute : System.Attribute
    {
        /// <summary>
        /// 数据库表、视图、存储过程的名称（映射）
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 是否开始缓存TableSet、ViewSet、ProcSet类型的数据
        ///// 开启后，会缓存整张表、视图
        ///// </summary>
        //public bool IsCache { get; set; }
    }
}
