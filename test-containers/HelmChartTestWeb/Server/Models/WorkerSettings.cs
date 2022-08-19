using Azihub.Utilities.Base.Tools.Annotations;

namespace HelmChartTestWeb.Server.Models
{
    public class WorkerSettings
    {
        public int DockerEnginePort { get; set; }
        public string DockerEngineHost { get; set; } = null!;
        public ushort DbServerPort { get; set; } = 1433;
        public string DbContainerBaseName { get; set; } = null!;
        public string DbName { get; set; } = null!;
        public string DbUsername { get; set; } = null!;
        public string DbPassword { get; set; } = null!;
        public bool DbTrustedConnection { get; set; }
        public bool DbMultipleActiveResultSets { get; set; }

        public string GetDbConnectionString(int port)
        {
            return $@"Data Source={DockerEngineHost},{port};" +
                   $"User ID={DbUsername};" +
                   $"Password={DbPassword};" +
                   $"Initial Catalog={DbName};" +
                   $"Persist Security Info=True;";
        }
    }
}