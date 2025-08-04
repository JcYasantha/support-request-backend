using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces.Service
{
    public interface ISupportRequestQueueService
    {
        bool QueueSupportRequest(SupportRequestSession supportRequest, bool isOfficeHours);
        SupportRequestSession? GetPriorityRequest();
        int MainQueueCount();
        int OverflowQueueCount();
    }
}
