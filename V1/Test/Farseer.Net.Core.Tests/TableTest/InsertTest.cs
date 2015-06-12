using System;
using Demo.PO;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class InsertTest
    {
        [TestMethod]
        public void Insert()
        {
            var count = Table.Data.User.Count();
            var currentCount = 0;
            UserVO info;
            using (var context = new Table())
            {
                info = new UserVO() { UserName = "xx" };
                context.User.Insert(info, true);
                context.SaveChanges();

                Assert.IsTrue(info.ID.GetValueOrDefault() > 0);

                info = context.User.Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info.UserName == "xx");

                currentCount = context.User.Count();
                Assert.IsTrue(currentCount == count + 1);
            }

            Table.Data.User.Insert(new UserVO() { UserName = "yy" });


            info = Table.Data.User.Desc(o => o.ID).ToEntity();
            Assert.IsTrue(info.UserName == "yy");

            currentCount = Table.Data.User.Count();
            Assert.IsTrue(currentCount == count + 2);

            // GUID
            Table.Data.Orders.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "1234567890", CreateAt = DateTime.Now, CreateID = 1, CreateName = "用户1" });
        }
    }
}
