using Moq;
using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;
using SupportRequest.Service;
using SupportRequest.Test.Helpers;

namespace SupportRequest.Test
{
    public class SupportRequestQueueServiceTest
    {
        private static ISupportRequestQueueService CreateQueueService(int mainLimit = 2, int overflowLimit = 1)
        {
            var repo = new Mock<ITeamsRepository>();
            var teamCapacityService = new Mock<ITeamCapacityService>();
            teamCapacityService.Setup(c => c.GetQueueLimit(It.IsAny<Team>())).Returns(mainLimit);
            teamCapacityService.Setup(c => c.GetOverFlowCapacity()).Returns(overflowLimit);

            var config = SupportRequestTestConfig.CreateTestConfig();

            return new SupportRequestQueueService(repo.Object, teamCapacityService.Object, config);
        }

        [Fact]
        public void QueueSupportRequest_WhenNotFull_EnqueueToMain()
        {
            //Arrange
            var queueService = CreateQueueService();
            var supportRequest = new SupportRequestSession();

            //Act
            bool result = queueService.QueueSupportRequest(supportRequest, true);

            //Assert
            Assert.True(result);
            Assert.Equal(1, queueService.MainQueueCount());
        }

        [Fact]
        public void QueueSupportRequest_WhenOverFlowAndOfficeHours_EnqueueToOverflow()
        {
            //Arrange
            var queueService = CreateQueueService(mainLimit: 1, overflowLimit: 2);
            queueService.QueueSupportRequest(new SupportRequestSession(), true); //This will fill the main queue
            var supportRequest = new SupportRequestSession();

            //Act
            bool result = queueService.QueueSupportRequest(supportRequest, true);

            //Assert
            Assert.True(result);
            Assert.Equal(1, queueService.OverflowQueueCount());

        }

        [Fact]
        public void QueueSupportRequest_WhenBothQueueFull_RefuseRequest()
        {
            //Arrange
            var queueService = CreateQueueService(mainLimit: 1, overflowLimit: 1);
            queueService.QueueSupportRequest(new SupportRequestSession(), true); //Fill main queue
            queueService.QueueSupportRequest(new SupportRequestSession(), true); //Fill overflow queue

            //Act
            var supportRequest = new SupportRequestSession();
            bool result = queueService.QueueSupportRequest(supportRequest, true);

            //Assert
            Assert.False(result);
            Assert.Equal(RequestStatus.Refused, supportRequest.Status);

        }

        [Fact]
        public void GetPriorityRequest_ShouldDequeueFromMainFirst()
        {
            //Arrange
            var queueService = CreateQueueService();
            var requstMain1 = new SupportRequestSession();
            var requstMain2 = new SupportRequestSession();

            queueService.QueueSupportRequest(requstMain1, true);
            queueService.QueueSupportRequest(requstMain2, true);

            //Act
            var result = queueService.GetPriorityRequest();

            //Assert
            Assert.Equal(requstMain1.Id, result?.Id);
        }

        [Fact]
        public void GetPriorityRequest_WhenMainQueueEmpty_ShouldDequeueFromOverflowQueue()
        {
            //Arrange
            var queueService = CreateQueueService(mainLimit: 1, overflowLimit: 1);
            var overflowRequest = new SupportRequestSession();

            queueService.QueueSupportRequest(new SupportRequestSession(), true); //Fill main
            queueService.QueueSupportRequest(overflowRequest, true); //Fill overflow
            queueService.GetPriorityRequest(); //Remove from main

            //Act
            var result = queueService.GetPriorityRequest();

            //Assert
            Assert.Equal(overflowRequest.Id, result?.Id);
        }

        [Fact]
        public void GetPriorityRequest_ShouldDequeueFromOverflowPerFairenessRule()
        {
            //Arrange
            var queueService = CreateQueueService(mainLimit: 5, overflowLimit: 1);

            for (int i = 0; i < 5; i++)
            {
                queueService.QueueSupportRequest(new SupportRequestSession(), true); //Fill main with 5 requests
            }

            var overflowRequest = new SupportRequestSession();
            queueService.QueueSupportRequest(overflowRequest, true); //Will add for overflow request

            for (int i = 0; i < 5; i++)
            {
                queueService.GetPriorityRequest(); //will dequeue main 5 items first
            }
           
            queueService.QueueSupportRequest(new SupportRequestSession(), true);  //Will add for the main queue again

            //Act
            var result = queueService.GetPriorityRequest();

            //Assert
            Assert.Equal(overflowRequest.Id, result?.Id);
        }

        [Fact]
        public void GetPriorityRequest_WhenBothQueuesFull_ReturnNull()
        {
            //Arrange
            var queueService = CreateQueueService(mainLimit: 1, overflowLimit: 1);

            //Act
            var result = queueService.GetPriorityRequest();

            //Assert
            Assert.Null(result);
        }
    }
}
