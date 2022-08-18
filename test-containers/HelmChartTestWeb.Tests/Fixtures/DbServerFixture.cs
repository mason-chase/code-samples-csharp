using Azihub.Utilities.Base.Tools;
using HelmChartTestWeb.Server.Models;
using System;
using System.Threading;
using HelmChartTestWeb.Server.Data;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Collections.Generic;

namespace HelmChartTestWeb.Tests.Fixtures
{
    public class DbServerFixture : IAsyncLifetime, IDisposable
    {
        private const int MsSqlServerDefaultPort = 1433;
        private WorkerSettings WorkerSettings { get; }

        private readonly TestcontainersContainer _testContainers;
        public ApplicationDbContext DbContext { get; }
        public int MsSqlServerContainerPublicPort { get; }
        private readonly string _msSqlServerContainerNameSuffix = Guid.NewGuid().ToString();
        public string MsSqlServerContainerName { get; }
        public string MsSqlServerHost { get; }

        public DbServerFixture()
        {
            DotEnv.Load(".env.test-container");

            WorkerSettings = DotEnv.Load<WorkerSettings>();
             MsSqlServerContainerName = $"{WorkerSettings.DbContainerBaseName}-{_msSqlServerContainerNameSuffix}";
             MsSqlServerHost = WorkerSettings.DockerEngineHost;
             var dockerEngineHost = WorkerSettings.DockerEngineHost;
             var dockerEnginePort = WorkerSettings.DockerEnginePort;
            
            _testContainers = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
              .WithName(MsSqlServerContainerName)
              .WithEnvironment("ACCEPT_EULA","y")
              .WithEnvironment("SA_PASSWORD", WorkerSettings.DbPassword)
              .WithEnvironment("MSSQL_SA_PASSWORD", WorkerSettings.DbPassword)
              .WithPortBinding(MsSqlServerDefaultPort, assignRandomHostPort: true)
              .WithCleanUp(true)
              .WithDockerEndpoint($"tcp://{dockerEngineHost}:{dockerEnginePort}")
              .Build();

            _testContainers.StartAsync().GetAwaiter().GetResult();

            MsSqlServerContainerPublicPort = _testContainers.GetMappedPublicPort(MsSqlServerDefaultPort);
            
            var result = _testContainers.ExecAsync(new List<string>
            {
                "ls", "-la"
            }).GetAwaiter().GetResult();

            result = _testContainers.ExecAsync(new List<string>
            {
                "sqlcmd"
            }).GetAwaiter().GetResult();
            DbContext = CreateContext();
        }

        public ApplicationDbContext CreateContext()
        {
            DbContextOptions<ApplicationDbContext> options = GetDbContextOptions();

            ApplicationDbContext context = new (options, GetOperationalStoreOptions());
            context.SaveChanges();

            return context;
        }

        private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            string connectionString = WorkerSettings.GetDbConnectionString(MsSqlServerContainerPublicPort);
            return new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseSqlServer(connectionString, builder =>
               {
                   builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
               }
               ).Options;
        }

        private static IOptions<OperationalStoreOptions> GetOperationalStoreOptions()
        {
            OperationalStoreOptions storeOptions = new();
            return Options.Create(storeOptions);
        }

        public void Dispose()
        {
            if (_testContainers is not null)
            {
                Task.Run( ()=>_testContainers.DisposeAsync());
            }

            GC.SuppressFinalize(this);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return _testContainers.DisposeAsync().AsTask();
        }
    }
}
