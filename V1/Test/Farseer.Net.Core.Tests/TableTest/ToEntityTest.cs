using System;
using System.Linq.Expressions;
using Demo.PO;
using Demo.VO.Members;
using FS.Extends;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class ToEntityTest
    {
        [TestMethod]
        public void ToEntity()
        {
            Table.Data.User.Select(o => o.ID).Where(1).ToEntity();
            Table.Data.User.Select(o => o.ID).Where(o => o.ID > 1).Where(1).ToEntity();

            var lst = Table.Data.User.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = Table.Data.User.Select(o => o.ID).Select(o => o.LoginCount).Where(o => o.ID > 1 || o.UserName.IsEquals("xx")).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = Table.Data.User.Select(o => new { o.ID, o.PassWord }).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<UserVO, object>> select = o => new { o.ID, o.PassWord };
            info = Table.Data.User.Select(select).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = Table.Data.User.Select(select).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void ToEntity_Top()
        {
            Table.Data.User.Select(o => o.ID).ToEntity(1);
        }
    }
}
