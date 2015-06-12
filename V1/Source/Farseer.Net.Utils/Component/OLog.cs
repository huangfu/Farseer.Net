using System;
using System.Threading;
using FS.Utils.Common;

namespace FS.Utils.Component
{
    /// <summary>
    /// 用于Console的输出
    /// </summary>
    public class OLog : IDisposable
    {
        public bool IsSuccess = true;
        private readonly Timer timer;
        private readonly ConsoleColor ErrorColor;

        public OLog(int millisecond = 500, ConsoleColor errorColor = ConsoleColor.Red)
        {
            timer = TimingTasks.Interval(o => { Console.Write("."); }, millisecond);
            ErrorColor = errorColor;
        }

        public static OLog Start(string log, ConsoleColor errorColor = ConsoleColor.Red, bool isAddTime = true, int millisecond = 500, string modify = "正在{0}")
        {
            var sp = new StrPlus();
            if (isAddTime) { sp.Append(string.Format("{0}\t", DateTime.Now.ToString("HH:mm:ss"))); }
            sp.Append(string.Format(modify + " ", log));
            Console.Write(sp);
            return new OLog(millisecond, errorColor);
        }

        public void Dispose()
        {
            timer.Dispose();
            if (IsSuccess) { Console.WriteLine("成功！"); }
            else
            {
                Console.ForegroundColor = ErrorColor;
                Console.WriteLine("失败！");
                Console.ForegroundColor = ConsoleColor.Gray;

            }
        }
    }
}
