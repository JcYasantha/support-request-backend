using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces.Service
{
    public interface ISupportRequestSessionService
    {
        SupportRequestSession? GetSession(Guid id);
        void CreateSession(SupportRequestSession supportRequestSession);
        bool AddToQueue(SupportRequestSession supportRequest);
        SupportRequestSession? UpdateLastPoll(Guid id);
    }
}
