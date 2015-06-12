using System;
using FS.Model.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Users.Data.Select(o => new {o.ID, o.LoginCount}).ToInfo();
        }
    }
}
