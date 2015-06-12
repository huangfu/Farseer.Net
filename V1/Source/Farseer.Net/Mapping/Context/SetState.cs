using FS.Mapping.Context.Attribute;

namespace FS.Mapping.Context
{
    /// <summary>
    /// 保存字段映射的信息
    /// </summary>
    public class SetState
    {
        /// <summary>
        /// Set特性
        /// </summary>
        public SetAttribute SetAtt { get; set; }

        /// <summary>
        ///     字段映射
        /// </summary>
        public FieldMap FieldMap { get; set; }
    }
}