using Demo.VO.Members;
using FS.Core.Data.View;
using FS.Mapping.Context.Attribute;

namespace Demo.PO
{
    [Context(0)]
    public class View : ViewContext<View>
    {
        public View() : base(0) { }

        [Set(Name = "View_Account")]
        public ViewSet<AccountVO> Account { get; set; }
    }
}