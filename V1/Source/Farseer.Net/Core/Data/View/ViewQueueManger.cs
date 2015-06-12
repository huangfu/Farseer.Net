using FS.Core.Infrastructure;
using FS.Mapping.Context;

namespace FS.Core.Data.View
{
    public class ViewQueueManger : BaseQueueManger
    {
        public ViewQueueManger(DbExecutor database, ContextMap contextMap)
            : base(database, contextMap) { }
    }
}
