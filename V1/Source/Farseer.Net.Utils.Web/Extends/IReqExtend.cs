using System;
using System.Collections.Generic;
using System.Web;
using FS.Core.Data.Proc;
using FS.Core.Data.Table;
using FS.Utils.Web.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class IReqExtend
    {
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TEntity Form<TEntity>(this TableSet<TEntity> entity, Action<Dictionary<string, List<string>>> tip, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.Form, tip, prefix);
        }
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TEntity Form<TEntity>(this TableSet<TEntity> entity, out Dictionary<string, List<string>> dicError, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.Form, out dicError, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TEntity Form<TEntity>(this ProcSet<TEntity> entity, Action<Dictionary<string, List<string>>> tip, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.Form, tip, prefix);
        }
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TEntity Form<TEntity>(this ProcSet<TEntity> entity, out Dictionary<string, List<string>> dicError, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.Form, out dicError, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TEntity QueryString<TEntity>(this TableSet<TEntity> entity, Action<Dictionary<string, List<string>>> tip, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.QueryString, tip, prefix);
        }
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TEntity QueryString<TEntity>(this TableSet<TEntity> entity, out Dictionary<string, List<string>> dicError, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.QueryString, out dicError, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TEntity QueryString<TEntity>(this ProcSet<TEntity> entity, Action<Dictionary<string, List<string>>> tip, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.QueryString, tip, prefix);
        }
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TEntity QueryString<TEntity>(this ProcSet<TEntity> entity, out Dictionary<string, List<string>> dicError, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.QueryString, out dicError, prefix);
        }
    }
}
