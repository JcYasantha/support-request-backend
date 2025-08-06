using SupportRequest.Core.Models;
using SupportRequest.Service;
using SupportRequest.Test.Helpers;

namespace SupportRequest.Test
{
    public class RoundRobinAssignmentStratergyTest
    {
        private Agent CreateAgent(SeniorityLevel level, int currentChats = 0)
        {
            return new Agent 
            {
                SeniorityLevel = level, 
                CurrentChats = currentChats,
                ShiftEndAt = DateTime.UtcNow.AddHours(1),
            };
        }

        [Fact]
        public void Assign_ShouldAssignToJuniorFirst()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var stratergy = new RoundRobinAssignmentStratergy(config);
            var supportRequestSession = new SupportRequestSession();

            var agents = new List<Agent> 
            { 
                CreateAgent(SeniorityLevel.Mid),
                CreateAgent(SeniorityLevel.Junior),
                CreateAgent(SeniorityLevel.Senior)
            };

            //Act
            bool assigned = stratergy.Assign(supportRequestSession, agents);

            //Assert
            Assert.True(assigned);
            Assert.Equal(SeniorityLevel.Junior, supportRequestSession?.AssignedAgent?.SeniorityLevel);
        }

        [Fact]
        public void Assign_WhenJuniorFull_ShouldAssignToMidWhenJuniorFull()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var stratergy = new RoundRobinAssignmentStratergy(config);
            var supportRequestSession = new SupportRequestSession();

            var agents = new List<Agent>
            {
                CreateAgent(SeniorityLevel.Mid),
                CreateAgent(SeniorityLevel.Junior, 4), //reached capacity for Junior (10 * 0.4)
                CreateAgent(SeniorityLevel.Senior)
            };

            //Act
            bool assigned = stratergy.Assign(supportRequestSession, agents);

            //Assert
            Assert.True(assigned);
            Assert.Equal(SeniorityLevel.Mid, supportRequestSession?.AssignedAgent?.SeniorityLevel);
        }

        [Fact]
        public void Assign_WhenJuniorAndMidFull_ShouldAssignToSenior()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var stratergy = new RoundRobinAssignmentStratergy(config);
            var supportRequestSession = new SupportRequestSession();

            var agents = new List<Agent>
            {
                CreateAgent(SeniorityLevel.Mid, 6), //reached capacity (10 * 0.6)
                CreateAgent(SeniorityLevel.Junior, 4), //reached capacity (10 * 0.4)
                CreateAgent(SeniorityLevel.Senior)
            };

            //Act
            bool assigned = stratergy.Assign(supportRequestSession, agents);

            //Assert
            Assert.True(assigned);
            Assert.Equal(SeniorityLevel.Senior, supportRequestSession?.AssignedAgent?.SeniorityLevel);
        }

        [Fact]
        public void Assign_WhenAllOthersFull_ShouldAssignToTeamLead()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var stratergy = new RoundRobinAssignmentStratergy(config);
            var supportRequestSession = new SupportRequestSession();

            var agents = new List<Agent>
            {
                CreateAgent(SeniorityLevel.Mid, 6), //reached capacity (10 * 0.6)
                CreateAgent(SeniorityLevel.Junior, 4), //reached capacity (10 * 0.4)
                CreateAgent(SeniorityLevel.Senior, 8), //reached capacity (10 * 0.8)
                CreateAgent(SeniorityLevel.TeamLead)
            };

            //Act
            bool assigned = stratergy.Assign(supportRequestSession, agents);

            //Assert
            Assert.True(assigned);
            Assert.Equal(SeniorityLevel.TeamLead, supportRequestSession?.AssignedAgent?.SeniorityLevel);
        }


        [Fact]
        public void Assign_WhenNoAvailableAgents_Returnsfalse()
        {
            //Arrange
            var config = SupportRequestTestConfig.CreateTestConfig();
            var stratergy = new RoundRobinAssignmentStratergy(config);
            var supportRequestSession = new SupportRequestSession();

            var juniorAgent = CreateAgent(SeniorityLevel.Junior, 4);
            juniorAgent.ShiftEndAt = DateTime.UtcNow.AddHours(-1);

            var agents = new List<Agent>
            {
               juniorAgent
            };

            //Act
            bool assigned = stratergy.Assign(supportRequestSession, agents);

            //Assert
            Assert.False(assigned);
            Assert.Null(supportRequestSession.AssignedAgent);
            Assert.Equal(RequestStatus.Queued, supportRequestSession.Status);
        }
    }
}
