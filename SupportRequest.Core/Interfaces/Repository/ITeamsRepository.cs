using SupportRequest.Core.Models;

namespace SupportRequest.Core.Interfaces.Repository
{
    public interface ITeamsRepository
    {
        List<Team> GetTeams();
        List<Agent> GetAgents();
        List<Agent> GetOverflowAgents();
        Team GetActiveTeam();
    }
}
