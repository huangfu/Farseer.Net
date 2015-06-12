using Demo.PO;
using Demo.VO.Members;
using FS.Utils.Component;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void TestTime()
        {
            SpeedTest.Initialize();
            var ID = Table.Data.User.Desc(o => o.ID).ToEntity().ID;
            Table.Data.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });

            SpeedTest.ConsoleTime("x1", 1, () =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    Table.Data.Set<UserVO>().Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
                }
            });
            SpeedTest.ConsoleTime("x2", 1, () =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    Table.Data.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
                }
            });


            //var context = new Table();
            //SpeedTest.ConsoleTime("批量提交", 1, () =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        context.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
            //    }
            //    context.SaveChanges();
            //});


            //SpeedTest.ConsoleTime("单次提交", 1, () =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Table.Data.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
            //    }
            //});
        }
    }
}
