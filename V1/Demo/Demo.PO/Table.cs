using Demo.VO.Members;
using FS.Core.Data.Table;
using FS.Mapping.Context.Attribute;

namespace Demo.PO
{
    /// <summary>
    /// 指定数据库配置项
    /// </summary>
    [Context(0)]
    public class Table : TableContext<Table>
    {
        [Set(Name = "Members_User")]
        public TableSet<UserVO> User { get; set; }

        [Set(Name = "Members_Role")]
        public TableSetCache<UserRoleVO> UserRole { get; set; }

        [Set(Name = "Members_Orders")]
        public TableSet<OrdersVO> Orders { get; set; }
    }
}