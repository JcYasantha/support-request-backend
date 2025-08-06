using SupportRequest.Core.Config;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;

namespace SupportRequest.Service
{
    public class RoundRobinAssignmentStratergy(SupportRequestConfig supportRequestConfig) : IAgentAssignmentStratergy
    {

        private readonly Dictionary<SeniorityLevel, int> priorityMap = new()
        {
            { SeniorityLevel.Junior, 1},
            { SeniorityLevel.Mid, 2},
            { SeniorityLevel.Senior, 3},
            { SeniorityLevel.TeamLead, 4}
        };

        public bool Assign(SupportRequestSession supportRequestSession, List<Agent> agents)
        {
            var orderedAgents = agents.OrderBy(agent => priorityMap[agent.SeniorityLevel]).ToList();

            foreach (var agent in orderedAgents) 
            {
                var capacity = supportRequestConfig.MaxConcurrency * supportRequestConfig.SeniorityMultipliers[agent.SeniorityLevel.ToString()];
                if (agent.IsAvailable(capacity))
                {
                    agent.CurrentChats++;
                    supportRequestSession.AssignedAgent = agent;
                    supportRequestSession.Status = RequestStatus.Active;
                    return true;
                }
            }
            return false;
        }
    }
}
