using Azihub.Utilities.Base.Tools.Annotations;

namespace HelmChartTestWeb.Server.Models
{
    public class WorkerSettings
    {
        public string DbServerHost { get; set; } = null!;
        public ushort DbServerPort { get; set; }
        public string DbContainerName { get; set; } = null!;
        public string DbName { get; set; } = null!;
        public string DbUsername { get; set; } = null!;
        public string DbPassword { get; set; } = null!;
        public bool DbTrustedConnection { get; set; }
        public bool DbMultipleActiveResultSets { get; set; }

        public string GetDbConnectionString()
        {
            return $@"Data Source={DbServerHost}\,{DbServerPort};" +
                   $"User ID={DbUsername};" +
                   $"Password={DbPassword};" +
                   $"Initial Catalog={DbName};" +
                   $"Persist Security Info=True;";
        }
    }
}