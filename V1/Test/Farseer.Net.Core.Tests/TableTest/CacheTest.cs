using System;
using Demo.PO;
using Demo.VO.Members;
using FS.Extends;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class CacheTest
    {
        [TestMethod]
        public void All()
        {
            var lst = Table.Data.UserRole.Cache;
            var count = lst.Count;
            var userCount = Table.Data.UserRole.Cache.GetValue(1, o => o.UserCount);
            using (var context = new Table())
            {
                //context.User.ToEntity();
                //context.User.ToList();
                //context.User.Insert(new UserVO() { UserName = "xx" }, true);


                context.UserRole.Insert(new UserRoleVO { Caption = "test", Descr = "cachetest" });
                context.UserRole.Where(o => o.Caption == "test").Update(new UserRoleVO { Caption = "testUpdate" });

                context.UserRole.AddUp(1, o => o.UserCount, 1);

                context.SaveChanges();
            }

            Table.Data.UserRole.AddUp(1, o => o.UserCount, 1);

            Assert.IsTrue(count + 1 == Table.Data.UserRole.Cache.Count, "缓存同步失败");
            Assert.IsTrue(userCount + 2 == Table.Data.UserRole.Cache.GetValue(1, o => o.UserCount), "缓存同步失败");
        }
    }
}
