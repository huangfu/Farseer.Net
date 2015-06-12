using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqLite
{
    public class ExpressionNew : DbExpressionNewProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public ExpressionNew(BaseQueueManger queueManger, Queue queue) : base(queueManger, queue) { }
    }
}
