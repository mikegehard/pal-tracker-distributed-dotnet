using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Backlog;
using DatabaseSupport;
using TestSupport;
using Xunit;

namespace BacklogTest
{
    [Collection("Backlog")]
    public class StoryDataGatewayTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static StoryDataGatewayTest()
        {
            TestEnvSupport.SetBacklogVcap();
        }

        public StoryDataGatewayTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestCreate()
        {
            var gateway = new StoryDataGateway(new DatabaseTemplate(DataSourceConfig));
            gateway.Create(22, "aStory");

            // todo...
            var template = new DatabaseTemplate(DataSourceConfig);
            var projectIds = template.Query("select project_id from stories", reader => reader.GetInt64(0),
                new List<DbParameter>());

            Assert.Equal(22, projectIds.First());
        }

        [Fact]
        public void TestFind()
        {
            Support.ExecSql("insert into stories (id, project_id, name) values (1346, 22, 'aStory');");

            var gateway = new StoryDataGateway(new DatabaseTemplate(DataSourceConfig));
            var list = gateway.FindBy(22);

            // todo...
            var actual = list.First();
            Assert.Equal(1346, actual.Id);
            Assert.Equal(22, actual.ProjectId);
            Assert.Equal("aStory", actual.Name);
        }
    }
}