using Demo.PO;
using FS.Extends;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class ToSelectListTest
    {
        [TestMethod]
        public void ToSelectList()
        {
            using (var context = new Table())
            {
                var where = context.User.Where(o => o.ID > 0 && o.ID != null).Asc(o => o.ID);
                where.Where(o => o.UserName.Contains("xx"));
                where.Where(o => o.ID > 1);
                where.Where(2).ToSelectList(o => o.ID.ConvertType(0));
            }
        }

        [TestMethod]
        public void ToSelectListTop()
        {
            using (var context = new Table())
            {
                var where = context.User.Where(o => o.ID > 0 && o.ID != null).Asc(o => o.ID);
                where.Where(o => o.UserName.Contains("xx"));
                where.Where(o => o.ID > 1);
                where.ToSelectList(2, o => o.ID.ConvertType(0));
            }
        }
    }
}
