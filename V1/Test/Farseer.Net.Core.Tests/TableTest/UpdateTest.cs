using Demo.PO;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class UpdateTest
    {
        [TestMethod]
        public void Update()
        {
            var ID = 0;
            using (var context = new Table())
            {
                ID = context.User.Desc(o => o.ID).ToEntity().ID.GetValueOrDefault();

                context.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
                context.SaveChanges();
                Assert.IsTrue(context.User.Desc(o => o.ID).ToEntity().UserName == "zz");
            }

            Table.Data.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(Table.Data.User.Desc(o => o.ID).ToEntity().UserName == "bb");

            Table.Data.User.Update(new UserVO() { UserName = "bb", ID = ID });
        }
    }
}
