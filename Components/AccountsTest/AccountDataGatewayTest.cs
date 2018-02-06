using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Accounts;
using DatabaseSupport;
using TestSupport;
using Xunit;

namespace AccountsTest
{
    [Collection("Accounts")]
    public class AccountDataGatewayTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static AccountDataGatewayTest()
        {
            TestEnvSupport.SetRegistrationVcap();
        }

        public AccountDataGatewayTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestCreate()
        {
            Support.ExecSql("insert into users (id, name) values (12, 'Jack');");

            var template = new DatabaseTemplate(DataSourceConfig);
            var gateway = new AccountDataGateway(template);
            gateway.Create(12, "anAccount");

            var names = template.Query("select name from accounts", reader => reader.GetString(0),
                new List<DbParameter>());

            Assert.Equal("anAccount", names.First());
        }

        [Fact]
        public void TestFindBy()
        {
            Support.ExecSql(@"
insert into users (id, name) values (12, 'Jack');
insert into accounts (id, owner_id, name) values (1, 12, 'anAccount'), (2, 12, 'anotherAccount');
");

            var gateway = new AccountDataGateway(new DatabaseTemplate(DataSourceConfig));

            var actual = gateway.FindBy(12);

            Assert.Equal(1, actual[0].Id);
            Assert.Equal(12, actual[0].OwnerId);
            Assert.Equal("anAccount", actual[0].Name);
            Assert.Equal(2, actual[1].Id);
            Assert.Equal(12, actual[1].OwnerId);
            Assert.Equal("anotherAccount", actual[1].Name);
        }
    }
}