using System.Data.Entity;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EntityFramework.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var po = new POTest())
            {
                //po.User.Where()
            }
        }
    }

    public class POTest : DbContext
    {
        public DbSet<UserVO> User { get; set; }
    }
}
