using Demo.PO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class AddUpTest
    {
        [TestMethod]
        public void AddUp()
        {
            using (var context = new Table())
            {
                var info = context.User.Desc(o => o.ID).ToEntity();

                context.User.Where(o => o.ID == info.ID).Append(o => new { o.LoginCount }, 4).AddUp();
                context.SaveChanges();
                var info2 = context.User.Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info2.LoginCount == info.LoginCount + 4);
            }
        }
    }
}
