using Azihub.Utilities.Base.Tools;
using HelmChartTestWeb.Server.Models;
using Xunit;

namespace HelmChartTestWeb.Tests
{
    public class WorkerSettingsTests
    {
        [Fact]
        public void GetWorkerSettingTests()
        {
            DotEnv.Load(".env.sample");

            WorkerSettings workerSettings = DotEnv.Load<WorkerSettings>();

            Assert.True(ValidateWorkerSettingsProperties(workerSettings));
        }

        private static bool ValidateWorkerSettingsProperties(WorkerSettings workerSettings)
        {
            Assert.Equal("127.0.0.1", workerSettings.DbServerHost);
            Assert.Equal("aspnet-HelmChartTestWeb", workerSettings.DbName);
            Assert.Equal("sa", workerSettings.DbUsername);
            Assert.Equal("JAsdfi7125o1ih2rt1", workerSettings.DbPassword);
            Assert.True(workerSettings.DbTrustedConnection);
            Assert.True(workerSettings.DbMultipleActiveResultSets);
            Assert.True(!string.IsNullOrWhiteSpace(workerSettings.GetDbConnectionString()));
            return true;
        }
    }
}