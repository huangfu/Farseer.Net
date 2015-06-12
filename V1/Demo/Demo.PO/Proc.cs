using Demo.VO.Members;
using FS.Core.Data.Proc;
using FS.Mapping.Context.Attribute;

namespace Demo.PO
{
    [Context(0)]
    public class Proc : ProcContext<Proc>
    {
        [Set(Name = "sp_Info_User")]
        public ProcSet<InfoUserVO> InfoUser { get; set; }

        [Set(Name = "sp_Insert_User")]
        public ProcSet<InsertUserVO> InsertUser { get; set; }

        [Set(Name = "sp_List_User")]
        public ProcSet<ListUserVO> ListUser { get; set; }

        [Set(Name = "sp_Value_User")]
        public ProcSet<ValueUserVO> ValueUser { get; set; }
    }
}
