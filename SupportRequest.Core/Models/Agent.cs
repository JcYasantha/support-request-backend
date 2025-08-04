namespace SupportRequest.Core.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public SeniorityLevel SeniorityLevel { get; set; }
        public int CurrentChats {  get; set; }
    }
}
