using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DatabaseSupport;
using Projects;
using TestSupport;
using Xunit;

namespace ProjectsTest
{
    [Collection("Projects")]
    public class ProjectDataGatewayTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static ProjectDataGatewayTest()
        {
            TestEnvSupport.SetRegistrationVcap();
        }

        public ProjectDataGatewayTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestCreate()
        {
            Support.ExecSql(@"
insert into users (id, name) values (12, 'Jack');
insert into accounts (id, owner_id, name) values (1, 12, 'anAccount');");

            var gateway = new ProjectDataGateway(new DatabaseTemplate(DataSourceConfig));
            gateway.Create(1, "aProject");

            // todo...
            var template = new DatabaseTemplate(DataSourceConfig);
            var projects = template.Query("select name from projects where account_id = 1",
                reader => reader.GetString(0),
                new List<DbParameter>());

            Assert.Equal("aProject", projects.First());
        }

        [Fact]
        public void TestFind()
        {
            Support.ExecSql(@"
insert into users (id, name) values (12, 'Jack');
insert into accounts (id, owner_id, name) values (1, 12, 'anAccount');
insert into projects (id, account_id, name) values (22, 1, 'aProject');");

            var gateway = new ProjectDataGateway(new DatabaseTemplate(DataSourceConfig));
            var list = gateway.FindBy(1);

            // todo...
            var actual = list.First();
            Assert.Equal(22, actual.Id);
            Assert.Equal(1, actual.AccountId);
            Assert.Equal("aProject", actual.Name);
            Assert.Equal(true, actual.Active);
        }

        [Fact]
        public void TestFindObject()
        {
            Support.ExecSql(@"
insert into users (id, name) values (12, 'Jack');
insert into accounts (id, owner_id, name) values (1, 12, 'anAccount');
insert into projects (id, account_id, name, active) values (22, 1, 'aProject', true);");

            var gateway = new ProjectDataGateway(new DatabaseTemplate(DataSourceConfig));
            var actual = gateway.FindObject(22);

            // todo...
            Assert.Equal(22, actual.Id);
            Assert.Equal(1, actual.AccountId);
            Assert.Equal("aProject", actual.Name);
            Assert.Equal(true, actual.Active);
        }
    }
}