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
            return $"Server={DbServerHost};"+
                $"Database={DbName};"+
                $"User Id={DbUsername};"+
                $"Password={DbPassword};" +
                $"Trusted_Connection={DbTrustedConnection};" +
                $"MultipleActiveResultSets={DbMultipleActiveResultSets};";
        }
    }
}
