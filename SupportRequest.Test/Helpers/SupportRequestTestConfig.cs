using SupportRequest.Core.Config;

namespace SupportRequest.Test.Helpers
{
    public static class SupportRequestTestConfig
    {
        public static SupportRequestConfig CreateTestConfig()
        {
            return new SupportRequestConfig
            {
                MaxConcurrency = 10,
                QueueMultiplier = 1.5,
                OverFlowAgents = 6,
                InactiveTimeoutSeconds = 3,
                SeniorityMultipliers = new Dictionary<string, double> {
                    {"Junior", 0.4}, {"Mid", 0.6}, {"Senior", 0.8}, {"TeamLead", 0.5}
                }
            };
        }
    }
}
