namespace SupportRequest.Core.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public RequestStatus ChatStatus { get; set; }
        public Agent? AssignedAgent { get; set; }
    }
}
