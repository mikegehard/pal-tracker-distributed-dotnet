using System;

namespace TestSupport
{
    public static class TestEnvSupport
    {
        private static void SetRegistrationVcap(string db)
        {
            var json = $@"
                {{
                    ""p-mysql"": [
                      {{
                            ""credentials"": {{
                                ""hostname"": ""localhost"",
                                ""port"": ""3306"",
                                ""name"": ""tracker_{db}_test"",
                                ""username"": ""tracker"",
                                ""password"": ""password""
                            }},
                            ""name"": ""tracker-{db}-database""
                        }}
                    ]
                }}";

            Environment.SetEnvironmentVariable("VCAP_SERVICES", json);
        }

        public static void SetRegistrationVcap() => SetRegistrationVcap("registration");
        public static void SetBacklogVcap() => SetRegistrationVcap("backlog");
        public static void SetAllocationsVcap() => SetRegistrationVcap("allocations");
        public static void SetTimesheetsVcap() => SetRegistrationVcap("timesheets");
    }
}