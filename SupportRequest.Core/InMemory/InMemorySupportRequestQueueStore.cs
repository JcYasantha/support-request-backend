using SupportRequest.Core.Models;

namespace SupportRequest.Core.InMemory
{
    public class InMemorySupportRequestQueueStore
    {
        public Queue<SupportRequestSession> MainQueue = new();
        public Queue<SupportRequestSession> OverflowQueue = new();
    }
}
