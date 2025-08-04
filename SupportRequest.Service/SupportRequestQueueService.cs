using SupportRequest.Core.Config;
using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;

namespace SupportRequest.Service
{
    public class SupportRequestQueueService(ITeamsRepository teamsRepository, ITeamCapacityService teamCapacityService, SupportRequestConfig supportRequestConfig) : ISupportRequestQueueService
    {
        private int mainDequeuedCount = 0;
        private readonly Queue<SupportRequestSession> mainQueue = new();
        private readonly Queue<SupportRequestSession> overflowQueue = new();

        public bool QueueSupportRequest(SupportRequestSession supportRequest, bool isOfficeHours)
        {
            var mainQueueLimit = teamCapacityService.GetQueueLimit(teamsRepository.GetActiveTeam());
            var overflowQueueLimit = teamCapacityService.GetOverFlowCapacity();
            if (mainQueue.Count < mainQueueLimit)
            {
                mainQueue.Enqueue(supportRequest);
                return true;
            }

            if (isOfficeHours && overflowQueue.Count < overflowQueueLimit)
            {
                overflowQueue.Enqueue(supportRequest);
                return true;
            }

            supportRequest.Status = RequestStatus.Refused;
            return false;
        }

        public SupportRequestSession? GetPriorityRequest()
        {
            if (mainQueue.Count > 0 && overflowQueue.Count > 0)
            {
                if (mainDequeuedCount >= supportRequestConfig.MainToOverflowRatio)
                {
                    mainDequeuedCount = 0;
                    return overflowQueue.Dequeue();
                }
                mainDequeuedCount++;
                return mainQueue.Dequeue();
            }

            if (mainQueue.Count > 0) { mainDequeuedCount++; return mainQueue.Dequeue(); }
            if (overflowQueue.Count > 0) { mainDequeuedCount = 0; return overflowQueue.Dequeue(); }
            return null;
        }

        public int MainQueueCount() => mainQueue.Count;
        public int OverflowQueueCount() => overflowQueue.Count;
    }
}
