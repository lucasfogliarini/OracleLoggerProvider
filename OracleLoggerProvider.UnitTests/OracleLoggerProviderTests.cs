using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace OracleLogging.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class OracleLoggerProviderTests
    {
        [Fact]
        public void OracleLoggerProvider_ShouldInit()
        {
            //Arrange
            var connectionString = "connectionString";
            var logConfiguration = NewLogConfiguration();

            //Act
            var oracleLoggerProvider = new OracleLoggerProvider(connectionString, logConfiguration);

            //Assert
            Assert.NotNull(oracleLoggerProvider);
        }

        [Fact]
        public void OracleLoggerProvider_ShouldException_WhenConnectionStringNull()
        {
            //Arrange
            var logConfiguration = new LogConfiguration("tableName");

            //Act
            Action action = () =>
            {
                var oracleLoggerProvider = new OracleLoggerProvider(null, logConfiguration);
            };

            //Assert
            Assert.Throws<ArgumentNullException>(OracleLoggerProvider.connectionStringParam, action);
        }

        [Fact]
        public void OracleLoggerProvider_ShouldException_WhenLogConfigurationNull()
        {
            //Arrange
            var connectionString = "connectionString";

            //Act
            Action action = () =>
            {
                var oracleLoggerProvider = new OracleLoggerProvider(connectionString, null);
            };

            //Assert
            Assert.Throws<ArgumentNullException>(OracleLoggerProvider.logConfigurationParam, action);
        }

        [Fact]
        public void OracleLoggerProvider_ShouldException_WhenLogParametersEmpty()
        {
            //Arrange
            var connectionString = "connectionString";
            var logConfiguration = new LogConfiguration("");

            //Act
            Action action = () =>
            {
                var oracleLoggerProvider = new OracleLoggerProvider(connectionString, logConfiguration);
            };

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(OracleLoggerProvider.logConfigurationParametersParam, action);
        }
        [Fact]
        public void CreateLogger_ShouldReturnLogger()
        {
            //Arrange
            var connectionString = "connectionString";
            var logConfiguration = NewLogConfiguration();
            var oracleLoggerProvider = new OracleLoggerProvider(connectionString, logConfiguration);

            //Act
            var logger = oracleLoggerProvider.CreateLogger("categoryNameCreatedAutomatically");

            //Assert
            Assert.NotNull(logger);
        }

        private LogConfiguration NewLogConfiguration(
            string tableName = "LOGGING.LOG",
            string serverName = "SERVER_NAME",
            string application = "APPLICATION",
            string date = "DATE",
            string level = "LEVEL",
            string sourceContext = "SOURCE_CONTEXT",
            string state = "STATE",
            string exception = "EXCEPTION"
            )
        {
            var logConfiguration = new LogConfiguration(tableName);
            logConfiguration.Add(serverName, "server1");
            logConfiguration.Add(application, "OracleLoggerProvider");
            logConfiguration.Add(date, LogValue.Date);
            logConfiguration.Add(level, LogValue.LogLevel);
            logConfiguration.Add(sourceContext, LogValue.SourceContext);
            logConfiguration.Add(state, LogValue.State);
            logConfiguration.Add(exception, LogValue.Exception);

            return logConfiguration;
        }
    }
}
