using System.Data.Common;
using System.Text;
using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }

        public override ISqlBuilder CreateSqlBuilder(BaseQueueManger queueManger, Queue queue)
        {
            switch (queueManger.ContextMap.ContextProperty.DataVer)
            {
                case "2000": return new SqlBuilder2000(queueManger, queue);
            }
            return new SqlBuilder(queueManger, queue);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(userID) && string.IsNullOrWhiteSpace(passWord)) { sb.Append(string.Format("Pooling=true;Integrated Security=True;")); }
            else { sb.Append(string.Format("User ID={0};Password={1};Pooling=true;", userID, passWord)); }

            sb.Append(string.Format("Data Source={0};Initial Catalog={1};", server, catalog));

            if (poolMinSize > 0) { sb.Append(string.Format("Min Pool Size={0};", poolMinSize)); }
            if (poolMaxSize > 0) { sb.Append(string.Format("Max Pool Size={0};", poolMaxSize)); }
            if (connectTimeout > 0) { sb.Append(string.Format("Connect Timeout={0};", connectTimeout)); }
            return sb.ToString();
        }
    }
}
