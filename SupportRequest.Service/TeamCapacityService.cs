using SupportRequest.Core.Config;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;

namespace SupportRequest.Service
{
    public class TeamCapacityService(SupportRequestConfig supportRequestConfig) : ITeamCapacityService
    {
        public double GetQueueLimit(Team team)
        {
            return team.Agents.Sum(agent => 
            {
                var efficiency = supportRequestConfig.SeniorityMultipliers[agent.SeniorityLevel.ToString()];
                return supportRequestConfig.MaxConcurrency * efficiency;
            });
        }

        public double GetTeamCapacity(Team team)
        {
            return GetQueueLimit(team) * supportRequestConfig.QueueMultiplier;
        }

        public double GetOverFlowCapacity()
        {
            return supportRequestConfig.OverFlowAgents * (supportRequestConfig.MaxConcurrency * supportRequestConfig.SeniorityMultipliers[SeniorityLevel.Junior.ToString()]);
        }
    }
}
