using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces.Service
{
    public interface IAgentAssignmentStratergy
    {
        bool Assign(SupportRequestSession supportRequestSession, List<Agent> agents);
    }
}
