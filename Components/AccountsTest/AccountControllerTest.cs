using System.Collections.Generic;
using Accounts;
using DatabaseSupport;
using Microsoft.AspNetCore.Mvc;
using TestSupport;
using Xunit;

namespace AccountsTest
{
    [Collection("Accounts")]
    public class AccountControllerTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static AccountControllerTest()
        {
            TestEnvSupport.SetRegistrationVcap();
        }

        public AccountControllerTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestGet()
        {
            Support.ExecSql("insert into users (id, name) values (4765, 'Jack'), (4766, 'Fred');");
            Support.ExecSql(@"insert into accounts (id, owner_id, name) 
            values (1673, 4765, 'Jack''s account'), (1674, 4765, 'Jack''s other account'), 
            (1675, 4766, 'Fred''s account');");

            var controller =
                new AccountController(new AccountDataGateway(new DatabaseTemplate(DataSourceConfig)));
            var result = controller.Get(4765);

            var info = (List<AccountInfo>) ((ObjectResult) result).Value;

            Assert.Equal(2, info.Count);
            Assert.Equal(1673, info[0].Id);
            Assert.Equal(4765, info[0].OwnerId);
            Assert.Equal("Jack's account", info[0].Name);
            Assert.Equal("account info", info[0].Info);
            Assert.Equal(1674, info[1].Id);
            Assert.Equal(4765, info[1].OwnerId);
            Assert.Equal("Jack's other account", info[1].Name);
            Assert.Equal("account info", info[1].Info);
        }
    }
}