using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces.Repository
{
    public interface ISupportRequestSessionRepository
    {
        void Add(SupportRequestSession supportRequestSession);
        IEnumerable<SupportRequestSession> GetAll();
        void Update(SupportRequestSession supportRequestSession);
    }
}
