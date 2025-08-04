using SupportRequest.Core.Models;
using SupportRequest.Service;
using SupportRequest.Test.Helpers;

namespace SupportRequest.Test
{
    public class TeamCapacityServiceTest
    {
        [Fact]
        public void GetTeamCapacity_WithConfigData_ReturnsExpectedCapacity()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var service = new TeamCapacityService(config);
            var team = new Team { 
                Agents = [ 
                    new() { SeniorityLevel = SeniorityLevel.Mid},
                    new() { SeniorityLevel = SeniorityLevel.Mid},
                    new() { SeniorityLevel = SeniorityLevel.Junior}
                ]
            };
            double expectedLimit = (2 * config.MaxConcurrency * config.SeniorityMultipliers[SeniorityLevel.Mid.ToString()]) +
                (1 * config.MaxConcurrency * config.SeniorityMultipliers[SeniorityLevel.Junior.ToString()]);
            double expectedCapacity = expectedLimit * config.QueueMultiplier;

            //Act
            double capacity = service.GetTeamCapacity(team);
            //Assert
            Assert.Equal(expectedCapacity, capacity);
        }

        [Fact]
        public void GetQueueLimit_WithConfigData_ReturnExpectedLimit()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var service = new TeamCapacityService(config);
            var team = new Team
            {
                Agents = [
                    new() { SeniorityLevel = SeniorityLevel.Mid},
                    new() { SeniorityLevel = SeniorityLevel.Mid},
                    new() { SeniorityLevel = SeniorityLevel.Junior}
                ]
            };
            double expectedLimit = (2 * config.MaxConcurrency * config.SeniorityMultipliers[SeniorityLevel.Mid.ToString()]) + 
                (1 * config.MaxConcurrency * config.SeniorityMultipliers[SeniorityLevel.Junior.ToString()]);
            
            //Act
            double limit = service.GetQueueLimit(team);
            //Assert
            Assert.Equal(expectedLimit, limit); 
        }

        [Fact]
        public void GetOverFlowCapacity_WithConfigData_ReturnsOverFLowCapacity()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var service = new TeamCapacityService(config);
            double expectedOverFlow = config.OverFlowAgents * (config.MaxConcurrency * config.SeniorityMultipliers[SeniorityLevel.Junior.ToString()]);
            
            //Act
            double overFlow = service.GetOverFlowCapacity();
            //Assert
            Assert.Equal(expectedOverFlow, overFlow);
        }
    }
}
