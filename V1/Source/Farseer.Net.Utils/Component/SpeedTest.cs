using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace FS.Utils.Component
{
    /// <summary>
    ///     测试效率的工具
    ///     用于做平均效率测试
    /// </summary>
    public  class SpeedTest : IDisposable
    {
        /// <summary>
        ///     锁定
        /// </summary>
        private readonly object objLock = new object();

        /// <summary>
        ///     保存测试的结果
        /// </summary>
        public List<SpeedResult> ListResult = new List<SpeedResult>();

        /// <summary>
        ///     保存测试的结果
        /// </summary>
        public SpeedResult Result
        {
            get { return ListResult.Last(); }
        }

        /// <summary>
        ///     使用完后，自动计算时间
        /// </summary>
        public void Dispose()
        {
            ListResult.Last(o => o.Timer.IsRunning).Timer.Stop();
        }

        /// <summary>
        ///     开始计数
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public SpeedTest Begin(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName ))
            {
                throw new Exception("必须设置keyName的值！");
            }

            Create(keyName);
            var result = ListResult.FirstOrDefault(o => o.KeyName == keyName);
            result.Timer = new Stopwatch();
            result.Timer.Start();
            return this;
        }

        /// <summary>
        ///     开始计数
        /// </summary>
        public SpeedTest Begin()
        {
            var result = new SpeedResult { Timer = new Stopwatch() };
            result.Timer.Start();

            ListResult = new List<SpeedResult> { result };
            return this;
        }

        /// <summary>
        ///     停止工作
        /// </summary>
        public void Stop(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName ))
            {
                throw new Exception("必须设置keyName的值！");
            }

            Create(keyName);
            ListResult.FirstOrDefault(o => o.KeyName == keyName).Timer.Stop();
        }

        /// <summary>
        ///     判断键位是否存在（不存在，自动创建）
        /// </summary>
        private void Create(string keyName)
        {
            if (ListResult.Count(o => o.KeyName == keyName) != 0) return;
            lock (objLock)
            {
                if (ListResult.Count(o => o.KeyName == keyName) == 0)
                {
                    ListResult.Add(new SpeedResult { KeyName = keyName, Timer = new Stopwatch() });
                }
            }
        }

        /// <summary>
        ///     返回执行结果
        /// </summary>
        public class SpeedResult
        {
            /// <summary>
            ///     当前键码
            /// </summary>
            public string KeyName;

            /// <summary>
            ///     当前时间计数器
            /// </summary>
            public Stopwatch Timer;
        }

        /// <summary>
        /// 初始化进程
        /// </summary>
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        /// <summary>
        /// 计算运行方法耗时情况
        /// </summary>
        /// <param name="name">本次计算的名称</param>
        /// <param name="iteration">计算次数</param>
        /// <param name="action">要计算的方法</param>
        public static void ConsoleTime(string name, int iteration, Action action)
        {
            if (string.IsNullOrWhiteSpace(name )) { return; }

            // 设置控制台前景色
            var currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 强制进去垃圾回收
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (var i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            // 开始计时
            var watch = new Stopwatch();
            watch.Start();
            var cycleCount = GetCycleCount();
            for (var i = 0; i < iteration; i++) action();
            var cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            // 输出计时结果
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
            Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));

            // 输出垃圾回收结果
            for (var i = 0; i <= GC.MaxGeneration; i++)
            {
                var count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t" + count);
            }

            Console.WriteLine();
        }

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();
    }
}