using System;
using System.Collections.Generic;
using System.Net.Http;
using DatabaseSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestSupport;
using Timesheets;
using Xunit;
using static TestSupport.TestServers;

namespace TimesheetsTest
{
    [Collection("Timesheets")]
    public class TimeEntryControllerTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        private readonly HttpClient _client = ActiveProjectServer.CreateClient();

        static TimeEntryControllerTest()
        {
            TestEnvSupport.SetTimesheetsVcap();
        }

        public TimeEntryControllerTest()
        {
            Support.TruncateAllTables();
        }

        private const string Sql = @"insert into time_entries (id, project_id, user_id, date, hours)
        values (1534, 55432, 4765, '2015-05-17', 5), (2534, 55432, 4765, '2015-05-18', 3);";

        [Fact]
        public void TestPost()
        {
            Support.ExecSql(Sql);

            var controller =
                new TimeEntryController(new TimeEntryDataGateway(new DatabaseTemplate(DataSourceConfig)),
                    new ProjectClient(_client, new LoggerFactory().CreateLogger<ProjectClient>()));

            var value = controller.Post(new TimeEntryInfo(-1, 55432, 4765, DateTime.Parse("2015-05-17"), 8, ""));
            var actual = (TimeEntryInfo) ((ObjectResult) value).Value;

            Assert.True(actual.Id > 0);
            Assert.Equal(55432, actual.ProjectId);
            Assert.Equal(4765, actual.UserId);
            Assert.Equal(17, actual.Date.Day);
            Assert.Equal(8, actual.Hours);
            Assert.Equal("entry info", actual.Info);
        }

        [Fact]
        public void TestGet()
        {
            Support.ExecSql(Sql);

            var controller =
                new TimeEntryController(new TimeEntryDataGateway(new DatabaseTemplate(DataSourceConfig)),
                    new ProjectClient(_client, new LoggerFactory().CreateLogger<ProjectClient>()));
            var result = controller.Get(4765);

            // todo...
            Assert.Equal(2, ((List<TimeEntryInfo>) ((ObjectResult) result).Value).Count);
        }
    }
}