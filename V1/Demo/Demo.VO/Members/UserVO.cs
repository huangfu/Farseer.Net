using System;
using Demo.Common;
using FS.Core.Infrastructure;
using FS.Mapping.Context.Attribute;

namespace Demo.VO.Members
{
    public class UserVO : IEntity<int?>
    {
        /// <summary> 用户ID </summary>
        [Field(IsPrimaryKey = true)]
        public int? ID { get; set; }
        /// <summary> 用户名 </summary>
        [Field()]
        public string UserName { get; set; }
        /// <summary> 密码 </summary>
        public string PassWord { get; set; }
        /// <summary> 会员类型 </summary>
        public eumGenderType? GenderType { get; set; }
        /// <summary> 登陆次数 </summary>
        public int? LoginCount { get; set; }
        /// <summary> 登陆IP </summary>
        public string LoginIP { get; set; }
        /// <summary> 登陆IP </summary>
        [Field(Name = "getdate()")]
        public DateTime? GetDate { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime? CreateAt { get; set; }
    }
}
