using Microsoft.AspNetCore.TestHost;

namespace TestSupport
{
    public static class TestServers
    {
        public static TestServer ActiveProjectServer =>
            IntegrationTestServer.Start(response: "{\"active\" : true }", port: 3001);
    }
}