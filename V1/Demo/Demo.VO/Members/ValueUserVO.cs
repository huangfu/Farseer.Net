using FS.Mapping.Context.Attribute;

namespace Demo.VO.Members
{
    public class ValueUserVO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Field(IsInParam = true)]
        public int? ID { get; set; }
    }
}