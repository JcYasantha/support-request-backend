using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces
{
    public interface ITeamCapacityService
    {
        double GetTeamCapacity(Team team);
        double GetQueueLimit(Team team);
        double GetOverFlowCapacity();
    }
}
