using System;
using DatabaseSupport;
using Xunit;

namespace DatabaseSupportTest
{
    public class VcapDataSourceConfigTest
    {
        [Fact]
        public void GetConnection()
        {
            Environment.SetEnvironmentVariable("VCAP_SERVICES", @"
            {
                ""p-mysql"": [
                    {
                        ""credentials"": {
                            ""hostname"": ""localhost"",
                            ""port"": ""3306"",
                            ""name"": ""a_test_database"",
                            ""username"": ""a_test_user"",
                            ""password"": ""a_test_password""
                        }
                    }
                ]
            }");

            var actual = new VcapDataSourceConfig().GetConnection().ConnectionString;
            Assert.Equal("Server=localhost;User Id=a_test_user;Password=a_test_password;Port=3306;Database=a_test_database", actual);
        }
    }
}