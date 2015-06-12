using FS.Core.Data;

namespace FS.Core.Infrastructure
{
    public interface IViewSet<TReturn>
    {
        Queue Queue { get; }
    }
}
