using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;

namespace SupportRequest.Service
{
    public class SupportRequestAssignmentService(IAgentAssignmentStratergy agentAssignmentStratergy) : ISupportRequestAssignmentService
    {
        public bool TryAgentAssign(SupportRequestSession supportRequestSession, List<Agent> agents) 
            => agentAssignmentStratergy.Assign(supportRequestSession, agents);
    }
}
