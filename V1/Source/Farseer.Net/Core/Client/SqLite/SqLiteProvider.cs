using System.Data.Common;
using System.Text;
using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqLite
{
    public class SqLiteProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SQLite"); }
        }
        public override ISqlBuilder CreateSqlBuilder(BaseQueueManger queueManger, Queue queue)
        {
            return new SqlBuilder(queueManger, queue);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer,int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};Min Pool Size={1};Max Pool Size={2};", GetFilePath(server), poolMinSize, poolMaxSize);
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.AppendFormat("Password={0};", passWord); }
            if (!string.IsNullOrWhiteSpace(dataVer)) { sb.AppendFormat("Version={0};", dataVer); }
            return sb.ToString();
        }
    }
}
