using Demo.PO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class StatisticsTest
    {
        [TestMethod]
        public void Sum()
        {
            var result = Table.Data.User.Sum(o => o.ID + o.LoginCount);
        }
    }
}