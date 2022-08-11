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
        private WorkerSettings WorkerSettings { get; }

        private readonly TestcontainersContainer _testcontainersBuilder;
        public ApplicationDbContext DbContext { get; }

        public DbServerFixture()
        {
            DotEnv.Load(".env.sample");

            WorkerSettings = DotEnv.Load<WorkerSettings>();

            _testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
              .WithName(WorkerSettings.DbContainerName+"-mason")
              .WithEnvironment("ACCEPT_EULA","y")
              .WithEnvironment("SA_PASSWORD", WorkerSettings.DbPassword)
              .WithEnvironment("MSSQL_SA_PASSWORD", WorkerSettings.DbPassword)
              .WithPortBinding(WorkerSettings.DbServerPort, 1433)
              .WithCleanUp(true)
              .WithDockerEndpoint("tcp://10.254.7.46:2375")
              .Build();

            _testcontainersBuilder.StartAsync().GetAwaiter().GetResult();

            var result = _testcontainersBuilder.ExecAsync(new List<string>
            {
                "ls", "-la"
            }).GetAwaiter().GetResult();

            result = _testcontainersBuilder.ExecAsync(new List<string>
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
            string connectionString = WorkerSettings.GetDbConnectionString();
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
            if (_testcontainersBuilder is not null)
            {
                Task.Run( ()=>_testcontainersBuilder.DisposeAsync());
            }

            GC.SuppressFinalize(this);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return _testcontainersBuilder.DisposeAsync().AsTask();
        }
    }
}
