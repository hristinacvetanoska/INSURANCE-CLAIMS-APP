namespace Claims.Tests.Auditing
{
    using Claims.Auditing;
    using Claims.Interfaces;
    using Moq;
    using Xunit;

    public class AuditerTests
    {
        [Fact]
        public async Task AuditClaimAsync_EnqueuesClaimMessage()
        {
            // Arrange
            var mockQueue = new Mock<IAuditQueue>();
            var sut = new Auditer(mockQueue.Object);

            // Act
            await sut.AuditClaimAsync("claim-1", "POST");

            // Assert
            mockQueue.Verify(q => q.Enqueue(It.Is<AuditMessage>(m =>
                m.EntityType == "Claim" &&
                m.EntityId == "claim-1" &&
                m.HttpRequestType == "POST")), Times.Once);
        }

        [Fact]
        public async Task AuditCoverAsync_EnqueuesCoverMessage()
        {
            // Arrange
            var mockQueue = new Mock<IAuditQueue>();
            var sut = new Auditer(mockQueue.Object);

            // Act
            await sut.AuditCoverAsync("cover-1", "DELETE");

            // Assert
            mockQueue.Verify(q => q.Enqueue(It.Is<AuditMessage>(m =>
                m.EntityType == "Cover" &&
                m.EntityId == "cover-1" &&
                m.HttpRequestType == "DELETE")), Times.Once);
        }

        [Fact]
        public async Task AuditClaimAsync_PropagatesQueueException()
        {
            // Arrange
            var mockQueue = new Mock<IAuditQueue>();
            mockQueue.Setup(q => q.Enqueue(It.IsAny<AuditMessage>())).Throws(new System.InvalidOperationException("queue failure"));
            var sut = new Auditer(mockQueue.Object);

            // Act / Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => sut.AuditClaimAsync("x", "POST"));
        }
    }
}