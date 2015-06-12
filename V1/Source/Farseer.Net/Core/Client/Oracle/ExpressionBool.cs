using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.Oracle
{
    public class ExpressionBool : Common.ExpressionBool
    {
        public ExpressionBool(BaseQueueManger queueManger, Queue queue) : base(queueManger, queue) { }
    }
}
