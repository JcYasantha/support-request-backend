using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Models;
using System.Collections.Concurrent;

namespace SupportRequest.Service.Repository
{
    public class InMemorySupportRequestSessionRepository : ISupportRequestSessionRepository
    {
        private readonly ConcurrentDictionary<Guid, SupportRequestSession> sessions = new();

        public void Add(SupportRequestSession supportRequestSession)
        {
            sessions.TryAdd(supportRequestSession.Id, supportRequestSession);
        }

        public IEnumerable<SupportRequestSession> GetAll()
        {
            return sessions.Values;
        }

        public void Update(SupportRequestSession supportRequestSession)
        {
            sessions[supportRequestSession.Id] = supportRequestSession;
        }
    }
}
