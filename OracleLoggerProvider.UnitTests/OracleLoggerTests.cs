using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace OracleLogging.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class OracleLoggerTests
    {
        [Fact]
        public void GenerateCommandText_ShouldReturnCommandText()
        {
            //Arrange
            var tableName = "LOGGING.LOG";
            var serverName = "SERVER_NAME";
            var application = "APPLICATION";
            var date = "DATE";
            var level = "LEVEL";
            var sourceContext = "SOURCE_CONTEXT";
            var state = "STATE";
            var exception = "EXCEPTION";
            var sb = new StringBuilder();
            sb.Append("begin ");
            sb.Append($"INSERT INTO {tableName}");
            sb.Append($"({serverName},{application},{date},{level},{sourceContext},{state},{exception})");
            sb.Append("VALUES");
            sb.Append($"(:{serverName},:{application},:{date},:{level},:{sourceContext},:{state},:{exception});");
            sb.Append(" end;");
            var expectedCommandText = sb.ToString();
            var expectedCategoryName = "OracleLoggerProvider";
            var logConfiguration = NewLogConfiguration(tableName, serverName, application, date, level, sourceContext, state, exception);
            OracleLogger logger = new OracleLogger(expectedCategoryName, null, logConfiguration);

            //Act
            var commandText = logger.GenerateCommandText();

            //Assert
            Assert.Equal(expectedCommandText, commandText);
        }

        [Fact]
        public void GetLogValues_ShouldReturnLogValues()
        {
            //Arrange
            var expectedCategoryName = "OracleLoggerProvider";
            var expectedLogLevel = LogLevel.Error;
            var expectedState = "An unexpected error has occurred.";
            var expectedException = new Exception(expectedState);
            var expectedDate = DateTime.Now.Date;
            EventId expectedEventId = 1;
            OracleLogger logger = new OracleLogger(expectedCategoryName, null, null);

            //Act
            var logValues = logger.GetLogValues(expectedLogLevel, expectedEventId, expectedState, expectedException);

            //Assert
            Assert.Equal(expectedCategoryName, logValues.SourceContext);
            Assert.Equal(expectedLogLevel, logValues.LogLevel);
            Assert.Equal(expectedDate, logValues.Date.Date);
            Assert.Equal(expectedEventId, logValues.EventId);
            Assert.Equal(expectedState, logValues.State);
            Assert.Equal(expectedException, logValues.Exception);
        }

        [Fact]
        public void GetValue_ShouldReturnValue_WhenInputAllLogValue()
        {
            var logValues = Enum.GetValues(typeof(LogValue));
            var log = NewLog();
            foreach (var logValueEnum in logValues)
            {
                var template = $":{logValueEnum.ToString()}";

                object value = log.GetValue(template);

                Assert.NotNull(value);
            }

            var expectedValue = "notLogValue";
            var notLogValue = log.GetValue(expectedValue);

            Assert.Equal(expectedValue, notLogValue);
        }

        [Fact]
        public void BeginScope_ShouldReturnDisposabler()
        {
            //Arrange
            var logger = new OracleLogger(null, null, null);

            //Act
            var scope = logger.BeginScope(1);

            //Assert
            Assert.NotNull(scope);
        }

        [Fact]
        public void IsEnabled_ShouldReturnAlwaysTrue_WhenAnyLogLevel()
        {
            //Arrange
            var logger = new OracleLogger(null, null, null);

            //Act
            var isEnabled = logger.IsEnabled(LogLevel.Warning);

            //Assert
            Assert.True(isEnabled);
        }

        [Fact]
        public void AppendParameters_ShouldReturnAlwaysTrue_WhenAnyLogLevel()
        {
            //Arrange
            var logConfiguration = NewLogConfiguration();
            var logger = new OracleLogger(null, null, logConfiguration);
            var oracleCommand = new OracleCommand();
            var log = NewLog();

            //Act
            logger.AppendParameters(oracleCommand, log);

            //Assert
            Assert.Equal(logConfiguration.Parameters.Count, oracleCommand.Parameters.Count);
        }

        private Log NewLog()
        {
            return new Log()
            {
                SourceContext = "SourceContext",
                EventId = 1,
                Exception = new ArgumentNullException(),
                LogLevel = LogLevel.Critical,
                State = "state",
                Date = DateTime.Now.Date
            };
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
