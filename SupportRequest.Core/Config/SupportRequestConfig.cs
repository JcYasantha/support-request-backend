namespace SupportRequest.Core.Config
{
    public class SupportRequestConfig
    {
        public double MaxConcurrency { get; set; }
        public double QueueMultiplier { get; set; }
        public int InactiveTimeoutSeconds { get; set; }
        public int OverFlowAgents { get; set; }
        public Dictionary<string, double> SeniorityMultipliers { get; set; } = [];
    }
}
