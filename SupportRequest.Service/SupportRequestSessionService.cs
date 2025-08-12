using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;
using SupportRequest.Service.Helpers;

namespace SupportRequest.Service
{
    public class SupportRequestSessionService(ISupportRequestQueueService supportRequestQueueService, 
        ISupportRequestSessionRepository supportRequestSessionRepository) : ISupportRequestSessionService
    {
        public SupportRequestSession? GetSession(Guid id)
        {
            return supportRequestSessionRepository.GetById(id);
        }

        public void CreateSession(SupportRequestSession supportRequestSession)
        {
            supportRequestSessionRepository.Add(supportRequestSession);
        }

        public bool AddToQueue(SupportRequestSession supportRequest)
        {
            var supportRequestSession = supportRequestQueueService.QueueSupportRequest(supportRequest, OfficeHoursHelper.IsOfficeHours());
            if (supportRequestSession.Status == RequestStatus.Refused)
                return false;
            supportRequestSessionRepository.Add(supportRequestSession);
            return true;
        }

        public SupportRequestSession? UpdateLastPoll(Guid id)
        {
            var session = supportRequestSessionRepository.GetById(id);
            if (session == null) return null;
            
            session.LastPollAt = DateTime.UtcNow;
            supportRequestSessionRepository.Update(session);
            return session;
        }
    }
}
