﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Allocations;
using DatabaseSupport;
using Microsoft.AspNetCore.Mvc;
using TestSupport;
using Xunit;
using static TestSupport.TestServers;

namespace AllocationsTest
{
    [Collection("Allocations")]
    public class AllocationsControllerTest
    {
        private static readonly IDataSourceConfig DataSourceConfig = new VcapDataSourceConfig();
        private static readonly TestDatabaseSupport Support = new TestDatabaseSupport(DataSourceConfig);

        private readonly HttpClient _client = ActiveProjectServer.CreateClient();

        static AllocationsControllerTest()
        {
            TestEnvSupport.SetAllocationsVcap();
        }

        public AllocationsControllerTest()
        {
            Support.TruncateAllTables();
        }

        [Fact]
        public void TestPost()
        {
            Support.ExecSql(@"insert into allocations (id, project_id, user_id, first_day, last_day) values 
                (754, 55432, 4765, '2015-05-17', '2015-05-18'), 
                (755, 55432, 4766, '2015-05-17', '2015-05-18');");

            var controller =
                new AllocationController(new AllocationDataGateway(new DatabaseTemplate(DataSourceConfig)),
                    new ProjectClient(_client));

            var value = controller.Post(new AllocationInfo(-1, 55432, 4765, DateTime.Parse("2014-05-16"),
                DateTime.Parse("2014-05-26"), ""));
            var actual = (AllocationInfo) ((ObjectResult) value).Value;

            Assert.True(actual.Id > 0);
            Assert.Equal(55432L, actual.ProjectId);
            Assert.Equal(4765L, actual.UserId);
            Assert.Equal(16, actual.FirstDay.Day);
            Assert.Equal(26, actual.LastDay.Day);
            Assert.Equal("allocation info", actual.Info);
        }

        [Fact]
        public void TestGet()
        {
            Support.ExecSql(@"insert into allocations (id, project_id, user_id, first_day, last_day) values 
                (754, 55432, 4765, '2015-05-17', '2015-05-18'), 
                (755, 55432, 4766, '2015-05-17', '2015-05-18');");

            var controller =
                new AllocationController(new AllocationDataGateway(new DatabaseTemplate(DataSourceConfig)), null);
            var result = controller.Get(55432);

            // todo...
            Assert.Equal(2, ((List<AllocationInfo>) ((ObjectResult) result).Value).Count);
        }
    }
}