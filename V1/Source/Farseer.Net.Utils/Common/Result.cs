using System.Collections.Generic;
using FS.Extends;
using FS.Mapping.Verify;

namespace FS.Utils.Common
{
    /// <summary>
    /// 返回结果情况
    /// </summary>
    public class Result
    {
        public Result()
        {
            LstErrMsg = new List<string>();
        }

        /// <summary>
        /// 是否出错
        /// </summary>
        public bool IsError { get { return LstErrMsg.Count > 0; } }

        /// <summary>
        /// 出错信息
        /// </summary>
        public List<string> LstErrMsg { get; set; }

        /// <summary>
        /// 出错代码
        /// </summary>
        public int ErrCode { get; set; }

        /// <summary>
        /// 添加出错消息
        /// </summary>
        /// <param name="dicError">出错消息</param>
        public void Add(Dictionary<string, List<string>> dicError)
        {
            foreach (var keyValue in dicError) { LstErrMsg.AddRange(keyValue.Value); }
        }

        /// <summary>
        /// 添加出错消息
        /// </summary>
        /// <param name="err">出错消息</param>
        /// <param name="format">Format格式化</param>
        public void Add(string err, params object[] format)
        {
            LstErrMsg.Add(string.Format(err, format));
        }

        /// <summary>
        /// 添加出错消息
        /// </summary>
        /// <param name="err">出错消息</param>
        /// <param name="format">Format格式化</param>
        /// <param name="isTrue">条件为真是，添加错误消息</param>
        public bool Add(bool isTrue, string err, params object[] format)
        {
            if (isTrue) { Add(err, format); }
            return isTrue;
        }

        /// <summary>
        /// 检测实体类是否符合特性要求
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="info">实体类</param>
        public bool Check<TEntity>(TEntity info) where TEntity : IVerification
        {
            // 检查实体情况
            Dictionary<string, List<string>> dicError;
            if (!info.Check(out dicError)) { Add(dicError); return false; }
            return true;
        }

        /// <summary>
        /// 清除错误消息
        /// </summary>
        public void Reset()
        {
            ErrCode = 0;
            LstErrMsg.Clear();
        }
    }
}
