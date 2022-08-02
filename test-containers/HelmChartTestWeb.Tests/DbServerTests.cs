using HelmChartTestWeb.Server.Data;
using HelmChartTestWeb.Server.Models;
using HelmChartTestWeb.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace HelmChartTestWeb.Tests
{
    public class DbServerTests : IClassFixture<WorkerSettingsFixture>, IClassFixture<DbServerFixture>
    {
        private DbServerFixture DbServerFixture { get; }
        private const int DEFAULT_IDENTITY_TABLE = 10;

        private readonly WorkerSettings _workerSettings;

        public DbServerTests(DbServerFixture dbServerFixture, WorkerSettingsFixture workerSettingsFixture)
        {
            DbServerFixture = dbServerFixture;
            _workerSettings = workerSettingsFixture.WorkerSettings;
        }


        /// <exception cref="Xunit.Sdk.TrueException"></exception>
        [Fact]
        public void TestMsSqlWithMigration_Success()
        {
            ApplicationDbContext dbContext = DbServerFixture.DbContext;

            dbContext.Database.Migrate();

            Assert.True(Connectable(dbContext));
            Assert.True(DbNameMatches(dbContext));
            Assert.True(TableCountMatches(dbContext));
            Debugger.Break();
        }

        private static bool Connectable(ApplicationDbContext dbContext)
        {
            Assert.True(dbContext.Database.CanConnect());
            return true;
        }

        private bool DbNameMatches(ApplicationDbContext dbContext)
        {
            string dbName = dbContext.Database.GetDbConnection().Database;
            Assert.Equal(_workerSettings.DbName, dbName);
            return true;
        }

        private static bool TableCountMatches(ApplicationDbContext dbContext)
        {
            var tables = dbContext.Model.GetEntityTypes()
                                        .Select(t => t.GetTableName())
                                        .Distinct()
                                        .ToList();
            Assert.Equal(DEFAULT_IDENTITY_TABLE, tables.Count);
            return true;
        }
    }
}