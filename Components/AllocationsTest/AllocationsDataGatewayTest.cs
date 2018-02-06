using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Allocations;
using DatabaseSupport;
using TestSupport;
using Xunit;

namespace AllocationsTest
{
    [Collection("Allocations")]
    public class AllocationDataGatewayTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        static AllocationDataGatewayTest()
        {
            TestEnvSupport.SetAllocationsVcap();
        }

        public AllocationDataGatewayTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestCreate()
        {            
            var gateway = new AllocationDataGateway(new DatabaseTemplate(DataSourceConfig));
            gateway.Create(22, 12, DateTime.Now, DateTime.Now);

            // todo...
            var template = new DatabaseTemplate(DataSourceConfig);
            var projectIds = template.Query("select project_id from allocations", reader => reader.GetInt64(0),
                new List<DbParameter>());

            Assert.Equal(22, projectIds.First());
        }

        [Fact]
        public void TestFind()
        {
            Support.ExecSql(@"insert into allocations 
(id, project_id, user_id, first_day, last_day) values (97336, 22, 12, now(), now());");

            var gateway = new AllocationDataGateway(new DatabaseTemplate(DataSourceConfig));
            var list = gateway.FindBy(22);

            // todo...
            var actual = list.First();
            Assert.Equal(97336, actual.Id);
            Assert.Equal(22, actual.ProjectId);
            Assert.Equal(12, actual.UserId);
        }
    }
}