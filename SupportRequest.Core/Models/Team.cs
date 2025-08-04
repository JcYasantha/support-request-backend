namespace SupportRequest.Core.Models
{
    public class Team
    {
        public string Name { get; set; } = string.Empty;
        public List<Agent> Agents { get; set; } = [];
        public DateTime ShiftStartAt { get; set; }
        public DateTime ShiftEndAt { get; set; }
    }
}
