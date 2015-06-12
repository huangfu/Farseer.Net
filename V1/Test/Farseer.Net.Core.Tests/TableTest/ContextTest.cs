using System;
using System.Linq;
using Demo.PO;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class TableContext1Test
    {
        [TestMethod]
        public void NewTableContextTestMethod()
        {
            using (var context = new Table()) { }

            new Table().User.Where(o => o.ID > 0).ToList();
            Table.Data.User.AddUp(o => o.LoginCount, 1);
            Table.Data.User.Where(o => o.ID > 0).ToList();
            Table.Data.Set<UserVO>().Where(o => o.ID > 0).ToList();
        }

        [TestMethod]
        public void NewAndSaveChangeTableContextTestMethod()
        {
            using (var context = new Table())
            {
                var info = context.User.Where(o => o.ID > 0 && o.CreateAt < DateTime.Now).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToEntity();
                info.PassWord = "77777";
                context.User.Where(o => o.ID == 1).Update(info);

                info.ID = null;
                info.PassWord = "00000New";
                context.User.Insert(info);


                context.User.Where(o => o.ID == 1).Append(o => o.LoginCount, 1).AddUp();
                context.User.AddUp(o => o.LoginCount, 1);
                context.UserRole.Cache.Where(o => o.ID == 1).ToList();
                context.UserRole.Cache.Where(o => o.ID > 1).ToList();
                var lst = context.User.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToList();

                context.SaveChanges();
            }
        }
    }
}