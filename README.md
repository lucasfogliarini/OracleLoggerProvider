### Install
[![NuGet](https://img.shields.io/nuget/v/OracleLoggerProvider.svg?&label=nuget%20OracleLoggerProvider)](https://www.nuget.org/packages/OracleLoggerProvider/)

### Setup
```csharp
//Startup.cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    string connectionString = "User Id=user; Password=pass; Data Source=datasource;";

    var logConfiguration = new LogConfiguration("LOGGING.LOG", LogLevel.Error);
    logConfiguration.Add("SERVER_NAME", "Server1");
    logConfiguration.Add("APPLICATION", "OracleLoggerProvider");
    logConfiguration.Add("DATE", LogValue.Date);
    logConfiguration.Add("LEVEL", LogValue.LogLevel);
    logConfiguration.Add("SOURCE_CONTEXT", LogValue.SourceContext);
    logConfiguration.Add("STATE", LogValue.State);
    logConfiguration.Add("EXCEPTION", LogValue.Exception);
    var oracleLoggerProvider = new OracleLogProvider(connectionString, logConfiguration);

    loggerFactory.AddProvider(oracleLoggerProvider);
}
```