using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces.Service
{
    public interface ISupportRequestAssignmentService
    {
        bool TryAgentAssign(SupportRequestSession supportRequestSession, List<Agent> agents);
    }
}
