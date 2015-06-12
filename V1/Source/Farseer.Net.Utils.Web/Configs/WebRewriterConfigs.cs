using System;
using System.Collections.Generic;
using FS.Utils;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> 重写地址规则 </summary>
    public class WebRewriterConfigs : AbsConfigs<WebRewriterConfig> { }

    /// <summary> 重写地址规则 </summary>
    [Serializable]
    public class WebRewriterConfig
    {
        /// <summary> 重写地址规则列表 </summary>
        public List<WebRewriterRule> Rules = new List<WebRewriterRule>();
    }

    /// <summary> 重写地址规则 </summary>
    public class WebRewriterRule
    {
        /// <summary> 请求地址 </summary>
        public string LookFor = "";
        /// <summary> 重写地址 </summary>
        public string SendTo = "";

        /// <summary> 通过索引返回实体 </summary>
        public static implicit operator WebRewriterRule(int index)
        {
            return WebRewriterConfigs.ConfigEntity.Rules.Count <= index ? null : WebRewriterConfigs.ConfigEntity.Rules[index];
        }
    }
}