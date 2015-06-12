using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Demo.Common;
using Demo.PO;
using Demo.VO.Members;
using FS.Extends;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class ToListTest
    {
        readonly List<int> _lstIDs = new List<int> { 1, 2, 3 };

        [TestMethod]
        public void ToList_Top()
        {
            using (var context = new Table())
            {
                // 取前十条 随机 非重复的数据
                Assert.IsTrue(context.User.Desc(o => o.ID).ToList(10, true, true).Count <= 10);
                // 取 随机 非重复的数据
                var IDValue = 0;
                context.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToList(0, true, true);
                // 取 随机 的数据
                context.User.ToList(0, true);
                // 取 非重复 的数据
                context.User.ToList(0, false, true);

                // 只取ID
                var ID = context.User.Select(o => new { o.ID }).ToList(1)[0].ID.GetValueOrDefault();
                // 筛选字段、条件、正序、倒序
                Expression<Func<UserVO, bool>> where = o => o.ID == ID;
                var lst = context.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(where).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
                Assert.IsTrue(lst.Count == 1 && lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LoginCount == null && lst[0].ID == ID);

                // 来一个复杂条件的数据
                context.User.Select(o => new { o.ID, o.PassWord, o.GetDate })
                    .Where(
                        o =>
                            o.ID == ID ||
                            o.LoginCount < 1 ||
                            o.CreateAt < DateTime.Now ||
                            o.CreateAt > DateTime.Now.AddDays(-365) ||
                            o.UserName.Contains("x") ||
                            o.UserName.StartsWith("x") ||
                            o.UserName.EndsWith("x") ||
                            o.UserName.Length > 0 ||
                            o.GenderType == eumGenderType.Man ||
                            !o.PassWord.Contains("x") ||
                            _lstIDs.Contains(o.ID) ||
                            new List<int>().Contains(o.ID))
                    .Desc(o => new { o.LoginCount, o.GenderType })
                    .Asc(o => o.ID)
                    .Desc(o => o.GetDate)
                    .ToList();

                // 取ID为：1、2、3 的数据
                context.User.Where(o => new List<int> { 1, 2, 3 }.Contains(o.ID)).ToList();

                Assert.IsTrue(context.User.Where(new List<int> { 1, 2, 3 }).ToList().Count <= 3);
                var count = 1;
                Assert.IsTrue(context.User.Where(_lstIDs).Where(o => (o.LoginCount & count) == count).ToList().Count <= 3);

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_PageSplit()
        {
            using (var context = new Table())
            {
                // 取第2页的数据（每页显示3条数据）
                Assert.IsTrue(context.User.ToList(3, 2).Count <= 3);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_PageSplit_RecordCount()
        {
            using (var context = new Table())
            {
                var recordCount = 0;
                var count = 1;
                // 取前99999条数据，并返回总数据
                Assert.IsTrue(context.User.Select(o => o.ID).Where(o => o.ID > 10).Where(o => (o.LoginCount & count) == count).ToList(99999, 1, out recordCount).Count == recordCount);
                context.User.Select(o => o.CreateAt).Where(o => o.ID > 10).Asc(o => o.ID).Desc(o => new { o.UserName, o.LoginCount }).ToList(10, 3, out recordCount);

                context.SaveChanges();
            }
        }
    }
}
