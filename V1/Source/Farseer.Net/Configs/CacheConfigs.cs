using System;
using FS.Utils;

namespace FS.Configs
{
    /// <summary> 缓存配置 </summary>
    public class CacheConfigs : AbsConfigs<CacheConfig> { }

    /// <summary> 缓存配置 </summary>
    [Serializable]
    public class CacheConfig
    {
        /// <summary> 缓存逻辑超时时间(分钟) </summary>
        public int CacheDbTimeOut = 1440;
        /// <summary> 缓存失效分钟 </summary>
        public int CacheTimeOut = 120;
        /// <summary> 数据在VisitTime分钟内被访问VisitCount次后开始缓存 </summary>
        public int VisitCount = 10;
        /// <summary> 数据在VisitTime分钟内被访问VisitCount次后开始缓存 </summary>
        public int VisitTime = 30;
    }
}