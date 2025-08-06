using Microsoft.Extensions.DependencyInjection;
using Moq;
using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;
using SupportRequest.Service.BackgroundServices;
using SupportRequest.Service.Helpers;

namespace SupportRequest.Test
{
    public class RequestAssignmentBackgroundServiceTest
    {
        private static RequestAssignmentBackgroundService CreateService(
            out Mock<ISupportRequestQueueService> mockSupportRequestQueueService,
            out Mock<ITeamsRepository> mockTeamsRepository,
            out Mock<ISupportRequestSessionRepository> mockSupportRequestSessionRepository,
            out Mock<ISupportRequestAssignmentService> mockAssigner)
        {
            mockSupportRequestQueueService = new Mock<ISupportRequestQueueService>();
            mockTeamsRepository = new Mock<ITeamsRepository>();
            mockSupportRequestSessionRepository = new Mock<ISupportRequestSessionRepository>();
            mockAssigner = new Mock<ISupportRequestAssignmentService>();

            var agent = new Agent 
            {
                Name = "Agent1",
                SeniorityLevel = SeniorityLevel.Junior,
                ShiftEndAt = DateTime.UtcNow.AddHours(1)
            };
            var team = new Team 
            { 
                Name = "TeamA",
                Agents = [agent]
            };
            mockTeamsRepository.Setup(r => r.GetActiveTeam()).Returns(team);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(ISupportRequestQueueService))).Returns(mockSupportRequestQueueService.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ITeamsRepository))).Returns(mockTeamsRepository.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ISupportRequestSessionRepository))).Returns(mockSupportRequestSessionRepository.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ISupportRequestAssignmentService))).Returns(mockAssigner.Object);

            var scope = new Mock<IServiceScope>();
            scope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);

            var scopeFactory = new Mock<IServiceScopeFactory>();
            scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

            return new RequestAssignmentBackgroundService(scopeFactory.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenRequestPresent_ShouldAssignAndSession()
        {
            //Arrange
            var service = CreateService(out var mockSupportRequestQueueService, out var mockTeamsRepository, out var mockSupportRequestSessionRepository, out var mockAssigner);
            var supportRequestSession = new SupportRequestSession();
            mockSupportRequestQueueService.Setup(service => service.GetPriorityRequest()).Returns(supportRequestSession);
            mockAssigner.Setup(a => a.TryAgentAssign(supportRequestSession, It.IsAny<List<Agent>>())).Returns(true);

            //Act
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(200); // Will stop after one iteration
            await service.StartAsync(cts.Token);

            //Assert
            mockSupportRequestQueueService.Verify(q => q.GetPriorityRequest(), Times.AtLeastOnce);
            mockAssigner.Verify(a => a.TryAgentAssign(supportRequestSession, It.IsAny<List<Agent>>()), Times.AtLeastOnce);
            mockSupportRequestSessionRepository.Verify(s => s.Add(supportRequestSession), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_WhenNoRequestPresent_ReturnsNull()
        {
            //Arrange
            var service = CreateService(out var mockSupportRequestQueueService, out var mockTeamsRepository, out var mockSupportRequestSessionRepository, out var mockAssigner);

            mockSupportRequestQueueService.Setup(service => service.GetPriorityRequest()).Returns((SupportRequestSession?)null);

            //Act
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(200); // Will stop after one iteration
            await service.StartAsync(cts.Token);

            //Assert
            mockAssigner.Verify(a => a.TryAgentAssign(It.IsAny<SupportRequestSession>(), It.IsAny<List<Agent>>()), Times.Never);
            mockSupportRequestSessionRepository.Verify(s => s.Add(It.IsAny<SupportRequestSession>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenMainQueueFullAndInOfficeHours_AssginOverflowAgents()
        {
            //Arrange
            OfficeHoursHelper.TimeProvider = () => new DateTime(2025, 1, 1, 9, 0, 0, DateTimeKind.Utc);
            var service = CreateService(out var mockSupportRequestQueueService, out var mockTeamsRepository, out var mockSupportRequestSessionRepository, out var mockAssigner);
            var supportRequestSession = new SupportRequestSession();
            mockSupportRequestQueueService.Setup(service => service.GetPriorityRequest()).Returns(supportRequestSession);
            mockAssigner.Setup(a => a.TryAgentAssign(supportRequestSession, It.IsAny<List<Agent>>())).Returns(true);
            mockTeamsRepository.Setup(r => r.GetOverflowAgents()).Returns([new Agent { Name = "AgentO1", SeniorityLevel = SeniorityLevel.Junior, ShiftEndAt = DateTime.UtcNow.AddHours(1) }]);

            //Act
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(200); // Will stop after one iteration
            await service.StartAsync(cts.Token);

            //Assert
            mockAssigner.Verify(a => a.TryAgentAssign(supportRequestSession, It.Is<List<Agent>>(list => list.Exists(agent => agent.Name == "AgentO1"))), Times.AtLeastOnce);

            OfficeHoursHelper.TimeProvider = () => DateTime.UtcNow;
        }
    }
}
