using FS.Mapping.Context.Attribute;

namespace Demo.VO.Members
{
    public class InfoUserVO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Field(IsInParam = true)]
        public int? ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
    }
}