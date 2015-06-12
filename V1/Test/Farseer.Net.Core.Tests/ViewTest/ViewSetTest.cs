using System;
using System.Linq;
using System.Linq.Expressions;
using Demo.PO;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.ViewTest
{
    [TestClass]
    public class ViewSetTest
    {
        [TestMethod]
        public void ToInfoTestMethod()
        {
            //View.Data.Account.SelectMany
            var lst = View.Data.Account.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = View.Data.Account.Select(o => o.ID).Select(o => o.Name).Where(o => o.ID > 1).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.Pwd == null && info.Name != null && info.ID != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = View.Data.Account.Select(o => new { o.ID, o.Pwd }).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<AccountVO, object>> select = o => new { o.ID, o.Pwd };
            info = View.Data.Account.Select(select).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = View.Data.Account.Select(select).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = View.Data.Account.Desc(o => o.ID).ToList(10, true, true);
            lst = View.Data.Account.ToList(0, true, true);
            lst = View.Data.Account.ToList();
            Assert.IsTrue(lst != null && lst.Count > 0);
            var ID = lst[0].ID.GetValueOrDefault();

            lst = View.Data.Account.Select(o => new { o.ID, o.Pwd, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.Name }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
            var info = lst[0];
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count == 1);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == ID);


            lst = View.Data.Account.ToList(3);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            lst = View.Data.Account.ToList(3, 2);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            var count = View.Data.Account.Where(o => o.ID > 10).Count();
            var recordCount = 0;
            lst = View.Data.Account.Where(o => o.ID > 10).ToList(99999, 1, out  recordCount).ToList();
            Assert.IsNotNull(lst);
            Assert.IsTrue(count == recordCount);
        }
    }
}