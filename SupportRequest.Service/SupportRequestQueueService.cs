using SupportRequest.Core.Config;
using SupportRequest.Core.InMemory;
using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;

namespace SupportRequest.Service
{
    public class SupportRequestQueueService(ITeamsRepository teamsRepository,
        ITeamCapacityService teamCapacityService, 
        SupportRequestConfig supportRequestConfig,
        InMemorySupportRequestQueueStore queueStore) : ISupportRequestQueueService
    {
        private int mainDequeuedCount = 0;

        public SupportRequestSession QueueSupportRequest(SupportRequestSession supportRequest, bool isOfficeHours)
        {
            var activeTeam = teamsRepository.GetActiveTeam();
            var mainQueueLimit = teamCapacityService.GetQueueLimit(activeTeam);
            var overflowQueueLimit = teamCapacityService.GetOverFlowCapacity();
            if (queueStore.MainQueue.Count < mainQueueLimit)
            {
                queueStore.MainQueue.Enqueue(supportRequest);
                return supportRequest;
            }

            if (isOfficeHours && queueStore.OverflowQueue.Count < overflowQueueLimit)
            {
                queueStore.OverflowQueue.Enqueue(supportRequest);
                return supportRequest;
            }

            supportRequest.Status = RequestStatus.Refused;
            return supportRequest;
        }

        public SupportRequestSession? GetPriorityRequest()
        {
            if (queueStore.MainQueue.Count > 0 && queueStore.OverflowQueue.Count > 0)
            {
                if (mainDequeuedCount >= supportRequestConfig.MainToOverflowRatio)
                {
                    mainDequeuedCount = 0;
                    return queueStore.OverflowQueue.Dequeue();
                }
                mainDequeuedCount++;
                return queueStore.MainQueue.Dequeue();
            }

            if (queueStore.MainQueue.Count > 0) { mainDequeuedCount++; return queueStore.MainQueue.Dequeue(); }
            if (queueStore.OverflowQueue.Count > 0) { mainDequeuedCount = 0; return queueStore.OverflowQueue.Dequeue(); }
            return null;
        }

        public int MainQueueCount() => queueStore.MainQueue.Count;
        public int OverflowQueueCount() => queueStore.OverflowQueue.Count;
    }
}
