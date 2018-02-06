using Accounts;
using DatabaseSupport;
using Microsoft.AspNetCore.Mvc;
using TestSupport;
using Users;
using Xunit;

namespace AccountsTest
{
    [Collection("Accounts")]
    public class RegistrationControllerTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static RegistrationControllerTest()
        {
            TestEnvSupport.SetRegistrationVcap();
        }

        public RegistrationControllerTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestPost()
        {
            Support.ExecSql("insert into users (id, name) values (4765, 'Jack'), (4766, 'Fred');");
            Support.ExecSql(@"insert into accounts (id, owner_id, name) 
            values (1673, 4765, 'Jack''s account'), (1674, 4766, 'Fred''s account');");

            var userDataGateway = new UserDataGateway(new DatabaseTemplate(DataSourceConfig));
            var accountDataGateway = new AccountDataGateway(new DatabaseTemplate(DataSourceConfig));
            var service = new RegistrationService(userDataGateway, accountDataGateway);

            var controller = new RegisationController(service);
            var value = controller.Post(new UserInfo(-1, "aUser", ""));
            var actual = (UserInfo) ((ObjectResult) value).Value;

            Assert.True(actual.Id > 0);
            Assert.Equal("aUser", actual.Name);
            Assert.Equal("registration info", actual.Info);
        }
    }
}