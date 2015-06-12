using System;
using System.IO;

namespace FS.Utils
{
    /// <summary>
    ///     配置管理工具
    /// </summary>
    public abstract class AbsConfigs<T> where T : class, new()
    {
        /// <summary>
        ///     锁对象
        /// </summary>
        private static object m_LockHelper = new object();

        /// <summary>
        ///     配置文件路径
        /// </summary>
        private static string filePath = SysMapPath.AppData;

        /// <summary>
        ///     配置文件名称
        /// </summary>
        private static string fileName;

        /// <summary>
        ///     配置变量
        /// </summary>
        protected static T m_ConfigEntity;

        /// <summary>
        ///     Config修改时间
        /// </summary>
        private static DateTime FileLastWriteTime;

        /// <summary>
        ///     加载配置文件的时间（60分钟重新加载）
        /// </summary>
        private static DateTime LoadTime;

        /// <summary>
        ///     获取配置文件所在路径
        /// </summary>
        private static string FileName
        {
            get
            {
                return fileName ?? (fileName = string.Format("{0}.config", typeof(T).Name.EndsWith("Config", true, null) ? typeof(T).Name.Substring(0, typeof(T).Name.Length - 6) : typeof(T).Name));
            }
        }

        /// <summary>
        ///     配置变量
        /// </summary>
        public static T ConfigEntity
        {
            get
            {
                if (m_ConfigEntity == null || ((DateTime.Now - LoadTime).TotalMinutes > 60 && FileLastWriteTime != File.GetLastWriteTime(filePath + FileName)))
                {
                    LoadConfig();
                }
                return m_ConfigEntity;
            }
        }

        /// <summary>
        ///     加载(反序列化)指定对象类型的配置对象
        /// </summary>
        public static void LoadConfig()
        {
            //不存在则自动接创建
            if (!File.Exists(filePath + FileName))
            {
                var t = new T();
                DynamicOperate.AddItem(t);
                SaveConfig(t);
            }
            FileLastWriteTime = File.GetLastWriteTime(filePath + FileName);

            lock (m_LockHelper)
            {
                m_ConfigEntity = Serialize.Load<T>(filePath,FileName);
                LoadTime = DateTime.Now;
            }
        }

        /// <summary>
        ///     保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="t">Config配置</param>
        public static void SaveConfig(T t = null)
        {
            if (t == null) { t = ConfigEntity; }
            Serialize.Save(t, filePath, FileName);
        }
    }
}