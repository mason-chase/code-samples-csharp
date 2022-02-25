using Azihub.Utilities.Base.Tools;
using HelmChartTestWeb.Server.Models;
using System;

namespace HelmChartTestWeb.Tests.Fixtures
{
    public class WorkerSettingsFixture : IDisposable
    {
        public WorkerSettingsFixture()
        {
            DotEnv.Load(".env.sample");
            WorkerSettings = DotEnv.Load<WorkerSettings>();
        }

        public WorkerSettings WorkerSettings { get; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
