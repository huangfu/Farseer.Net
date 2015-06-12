using System.Linq;
using Demo.PO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class GetValueTest
    {

        [TestMethod]
        public void GetValue()
        {
            Table.Data.User.Where(o => o.ID > 1 && o.ID > 2).GetValue(o => o.UserName).ToList();
        }
    }
}
