using System.ComponentModel.DataAnnotations;
using FS.Mapping.Context.Attribute;

namespace FS.Mapping.Context
{
    public class FieldState
    {
        /// <summary>
        ///     数据类型
        /// </summary>
        public DataTypeAttribute DataType { get; set; }

        /// <summary>
        ///     字段映射
        /// </summary>
        public FieldAttribute FieldAtt { get; set; }

        /// <summary>
        ///     扩展类型
        /// </summary>
        public eumPropertyExtend PropertyExtend { get; set; }
    }

    /// <summary>
    ///     属性类型，自定义扩展属性
    /// </summary>
    public enum eumPropertyExtend
    {
        /// <summary>
        ///     Xml属性
        /// </summary>
        Attribute,

        /// <summary>
        ///     Xml节点
        /// </summary>
        Element,
    }
}
