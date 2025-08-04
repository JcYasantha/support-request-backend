using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Models;

namespace SupportRequest.Service.Repository
{
    public class InMemoryTeamRepository : ITeamsRepository
    {
        private readonly List<Team> _teams;
        public InMemoryTeamRepository()
        {
            var today = DateTime.UtcNow.Date;

            _teams =
            [
                new Team {
                    Name = "TeamC",
                    ShiftStartAt = today.AddHours(0),
                    ShiftEndAt   = today.AddHours(8),
                    Agents = CreateAgents("TeamC", [SeniorityLevel.Mid, SeniorityLevel.Mid], today.AddHours(8))
                },
                new Team {
                    Name = "TeamA",
                    ShiftStartAt = today.AddHours(8),
                    ShiftEndAt   = today.AddHours(16),
                    Agents = CreateAgents("TeamA", [SeniorityLevel.TeamLead, SeniorityLevel.Mid, SeniorityLevel.Mid, SeniorityLevel.Junior], today.AddHours(16))
                },
                new Team {
                    Name = "TeamB",
                    ShiftStartAt = today.AddHours(16),
                    ShiftEndAt   = today.AddDays(1).AddHours(0),
                    Agents = CreateAgents("TeamB", [SeniorityLevel.Senior, SeniorityLevel.Mid, SeniorityLevel.Junior, SeniorityLevel.Junior], today.AddDays(1).AddHours(0))
                }
            ];
        }

        public Team GetActiveTeam()
        {
            var now = DateTime.UtcNow;
            return _teams.FirstOrDefault(t => now >= t.ShiftStartAt && now < t.ShiftEndAt) ?? _teams.First();
        }

        public List<Agent> GetAgents() => [.. _teams.SelectMany(t => t.Agents)];

        public List<Agent> GetOverflowAgents() =>
            [.. Enumerable.Range(1, 6).Select(i => new Agent
            {
                Id = Guid.NewGuid(),
                Name = $"OverflowJr{i}",
                SeniorityLevel = SeniorityLevel.Junior,
                ShiftEndAt = DateTime.UtcNow.AddHours(24)
            })];

        public List<Team> GetTeams() => _teams;

        private static List<Agent> CreateAgents(string prefix, SeniorityLevel[] levels, DateTime shiftEnd)
        {
            var list = new List<Agent>();
            for (int i = 0; i < levels.Length; i++)
                list.Add(new Agent { Id = Guid.NewGuid(), Name = $"{prefix}_Agent{i + 1}", SeniorityLevel = levels[i], ShiftEndAt = shiftEnd });
            return list;
        }
    }
}
