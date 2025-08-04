namespace SupportRequest.Core.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public SeniorityLevel SeniorityLevel { get; set; }
        public int CurrentChats {  get; set; }
        public DateTime ShiftEndAt { get; set; }
    }
}
