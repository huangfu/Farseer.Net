using System;
using System.Diagnostics;
using System.Web;

namespace FS.Utils.Web.Common
{
    /// <summary>
    ///     测试效率的工具
    ///     用于做平均效率测试
    /// </summary>
    public class SpeedTest
    {
        public static string WebTime(string key, int count, Action act, bool outPut = true)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < count; i++) act();
            watch.Stop();
            var result = string.Format("{0}: {1}ms", key, watch.ElapsedMilliseconds);
            if (outPut) { HttpContext.Current.Response.Write(result + "<br />"); }
            return result;
        }
    }
}