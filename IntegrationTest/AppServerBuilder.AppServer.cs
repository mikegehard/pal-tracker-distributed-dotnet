using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using DatabaseSupport;
using TestSupport;

namespace IntegrationTest
{
    public partial class AppServerBuilder
    {
        public class AppServer
        {
            private readonly IDictionary<string, string> _environmentVariables;
            private readonly string _database;
            private readonly string _baseUrl;
            private readonly Process _process;

            private const string DbUsername = "tracker";
            private const string DbPassword = "password";

            internal AppServer(int port, string appName, IDictionary<string, string> environmentVariables, string database)
            {
                _baseUrl = $"http://localhost:{port}";
                _environmentVariables = environmentVariables;
                _database = database;

                var appPath = $"{AppContext.BaseDirectory}../../../../Applications/{appName}";

                _process = new Process
                {
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = $"exec {appPath}/bin/Debug/netcoreapp2.0/{appName}.dll --urls {_baseUrl}",
                        UseShellExecute = false,
                        WorkingDirectory = appPath
                    }
                };
            }

            public string Url(string relativePath = "/") => $"{_baseUrl}{relativePath}";

            public void Start()
            {
                foreach (var entry in _environmentVariables)
                {
                    Environment.SetEnvironmentVariable(entry.Key, entry.Value);
                }

                ConfigureTestDbVcap(_database);

                // This fails silently if the process is unable to start
                // (e.g., bad dll name)
                _process.Start();
                WaitUntilReady();

                var dbSupport = new TestDatabaseSupport(new VcapDataSourceConfig());
                dbSupport.TruncateAllTables();
            }

            public void Stop()
            {
                _process.Kill();
            }

            private void WaitUntilReady()
            {
                const int retryThreshold = 6;
                const int delay = 1000;
                var httpClient = new HttpClient();

                var tries = 0;
                Exception failureReason = null;

                while (tries < retryThreshold)
                {
                    try
                    {
                        if (httpClient.GetAsync(_baseUrl).Result.IsSuccessStatusCode)
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        failureReason = ex;
                        Thread.Sleep(delay);
                    }

                    tries++;
                }

                throw failureReason;
            }

            private static void ConfigureTestDbVcap(string database)
            {
                var json = $@"
                {{
                    ""p-mysql"": [
                      {{
                            ""credentials"": {{
                                ""hostname"": ""localhost"",
                                ""port"": ""3306"",
                                ""name"": ""{database}"",
                                ""username"": ""{DbUsername}"",
                                ""password"": ""{DbPassword}""
                            }},
                            ""name"": ""tracker-{database}-database""
                        }}
                    ]
                }}";

                Environment.SetEnvironmentVariable("VCAP_SERVICES", json);
            }
        }
    }
}