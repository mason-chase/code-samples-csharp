# Integrations Test with Helm Chart


Run SQL Test:

```
set REPLY=JAsdfi7125o1ih2rt1
docker exec -it mssql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P %REPLY%
```

SQL-CMD Tests:
```
SELECT Name from sys.Databases
GO

SELECT * FROM aspnet-HelmChartTestWeb.INFORMATION_SCHEMA.TABLES WHERE  TABLE_TYPE = 'BASE TABLE';
GO

```

