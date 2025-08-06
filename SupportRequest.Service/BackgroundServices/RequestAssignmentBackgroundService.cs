using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Service.Helpers;

namespace SupportRequest.Service.BackgroundServices
{
    public class RequestAssignmentBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                using var scope = scopeFactory.CreateScope();

                var supportRequestQueueService = scope.ServiceProvider.GetRequiredService<ISupportRequestQueueService>();
                var teamsRepository = scope.ServiceProvider.GetRequiredService<ITeamsRepository>();
                var supportRequestSessionRepository = scope.ServiceProvider.GetRequiredService<ISupportRequestSessionRepository>();
                var assigner = scope.ServiceProvider.GetRequiredService<ISupportRequestAssignmentService>();

                var priorityRequest = supportRequestQueueService.GetPriorityRequest();
                if (priorityRequest != null)
                {
                    var agents = teamsRepository.GetActiveTeam().Agents;
                    if (OfficeHoursHelper.IsOfficeHours())
                        agents.AddRange(teamsRepository.GetOverflowAgents());

                    assigner.TryAgentAssign(priorityRequest, agents);
                    supportRequestSessionRepository.Add(priorityRequest);
                }
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
