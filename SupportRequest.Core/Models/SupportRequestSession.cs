namespace SupportRequest.Core.Models
{
    public class SupportRequestSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.Queued;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastPollAt { get; set; } = DateTime.UtcNow;
        public Agent? AssignedAgent { get; set; }
    }
}
