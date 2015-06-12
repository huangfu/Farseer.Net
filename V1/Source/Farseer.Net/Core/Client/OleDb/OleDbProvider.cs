using System.Data.Common;
using System.Text;
using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb
{
    public class OleDbProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OleDb"); }
        }
        public override ISqlBuilder CreateSqlBuilder(BaseQueueManger queueManger, Queue queue)
        {
            return new SqlBuilder(queueManger, queue);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            switch (dataVer)
            {
                case "3.0": { sb.Append(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;")); break; }//Extended Properties=Excel 3.0;
                case "4.0": { sb.Append(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;")); break; }//Extended Properties=Excel 4.0;
                case "5.0": { sb.Append(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;")); break; }//Extended Properties=Excel 5.0;
                case "95": { sb.Append(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;")); break; }//Extended Properties=Excel 5.0;
                case "97": { sb.Append(string.Format("Provider=Microsoft.Jet.OLEDB.3.51;")); break; }
                case "2003": { sb.Append(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;")); break; }//Extended Properties=Excel 8.0;
                //  2007+   DR=YES
                default: { sb.Append(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;")); break; }//Extended Properties=Excel 12.0;
            }
            sb.Append(string.Format("Data Source={0};", GetFilePath(server)));
            if (!string.IsNullOrWhiteSpace(userID)) { sb.Append(string.Format("User ID={0};", userID)); }
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.Append(string.Format("Password={0};", passWord)); }

            return sb.ToString();
        }
    }
}
