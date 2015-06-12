using FS.Mapping.Context.Attribute;

namespace Demo.VO.Members
{
    public class AccountVO
    {
        /// <summary> 用户ID </summary>
        [Field(IsPrimaryKey = true)]
        public int? ID { get; set; }
        /// <summary> 用户名 </summary>
        public string Name { get; set; }
        /// <summary> 密码 </summary>
        public string Pwd { get; set; }
        /// <summary> 登陆IP </summary>
        [Field(Name = "getdate()")]
        public string GetDate { get; set; }
    }
}