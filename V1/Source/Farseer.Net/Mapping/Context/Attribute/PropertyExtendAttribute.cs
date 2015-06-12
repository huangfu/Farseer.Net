using System;

namespace FS.Mapping.Context.Attribute
{
    /// <summary>
    ///     设置变量的扩展属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyExtendAttribute : System.Attribute
    {
        /// <summary>
        ///     设置变量的扩展属性
        /// </summary>
        public PropertyExtendAttribute(eumPropertyExtend propertyExtend)
        {
            PropertyExtend = propertyExtend;
        }

        /// <summary>
        ///     设置变量的扩展属性
        /// </summary>
        internal eumPropertyExtend PropertyExtend { get; set; }
    }
}
