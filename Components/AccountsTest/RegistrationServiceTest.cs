using Accounts;
using DatabaseSupport;
using TestSupport;
using Users;
using Xunit;

namespace AccountsTest
{
    [Collection("Accounts")]
    public class RegistrationServiceTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static RegistrationServiceTest()
        {
            TestEnvSupport.SetRegistrationVcap();
        }

        public RegistrationServiceTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestFindBy()
        {
            var userDataGateway = new UserDataGateway(new DatabaseTemplate(DataSourceConfig));
            var accountDataGateway = new AccountDataGateway(new DatabaseTemplate(DataSourceConfig));
            var service = new RegistrationService(userDataGateway, accountDataGateway);

            var info = service.CreateUserWithAccount("aUser");

            Assert.Equal("aUser", info.Name);
        }
    }
}