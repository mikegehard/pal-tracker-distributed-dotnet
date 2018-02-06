using DatabaseSupport;
using Microsoft.AspNetCore.Mvc;
using TestSupport;
using Users;
using Xunit;

namespace UsersTest
{
    [Collection("Users")]
    public class UserControllerTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static UserControllerTest()
        {
            TestEnvSupport.SetRegistrationVcap();
        }

        public UserControllerTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestGet()
        {
            Support.ExecSql("insert into users (id, name) values (4765, 'Jack'), (4766, 'Fred');");

            var controller =
                new UserController(new UserDataGateway(new DatabaseTemplate(DataSourceConfig)));
            var result = controller.Get(4765);
            var info = ((UserInfo) ((ObjectResult) result).Value);

            Assert.Equal(4765, info.Id);
            Assert.Equal("Jack", info.Name);
            Assert.Equal("user info", info.Info);
        }
    }
}