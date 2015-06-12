using System;

namespace FS.Utils
{
    /// <summary>
    /// 获取系统路径
    /// </summary>
    public static class SysMapPath
    {
        /// <summary>
        /// 获取根目录下App_Data的路径
        /// </summary>
        public static string AppData
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\"; }
        }
    }
}
