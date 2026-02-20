namespace Claims.Tests.Auditing
{
    using Claims.Auditing;
    using Xunit;

    public class AuditQueueTests
    {
        [Fact]
        public async Task Enqueue_SingleMessage_ReaderReceivesMessage()
        {
            // Arrange
            var queue = new AuditQueue();
            var message = new AuditMessage
            {
                EntityId = "claim-1",
                EntityType = "Claim",
                HttpRequestType = "POST"
            };

            // Act
            queue.Enqueue(message);

            // Assert
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var received = await queue.Reader.ReadAsync(cts.Token);
            Assert.NotNull(received);
            Assert.Equal(message.EntityId, received.EntityId);
            Assert.Equal(message.EntityType, received.EntityType);
            Assert.Equal(message.HttpRequestType, received.HttpRequestType);
        }

        [Fact]
        public async Task Enqueue_MultipleMessages_OrderPreserved()
        {
            // Arrange
            var queue = new AuditQueue();
            var msg1 = new AuditMessage { EntityId = "m1", EntityType = "Claim", HttpRequestType = "POST" };
            var msg2 = new AuditMessage { EntityId = "m2", EntityType = "Cover", HttpRequestType = "DELETE" };
            var msg3 = new AuditMessage { EntityId = "m3", EntityType = "Claim", HttpRequestType = "PUT" };

            // Act
            queue.Enqueue(msg1);
            queue.Enqueue(msg2);
            queue.Enqueue(msg3);

            // Assert
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var r1 = await queue.Reader.ReadAsync(cts.Token);
            var r2 = await queue.Reader.ReadAsync(cts.Token);
            var r3 = await queue.Reader.ReadAsync(cts.Token);

            Assert.Equal("m1", r1.EntityId);
            Assert.Equal("m2", r2.EntityId);
            Assert.Equal("m3", r3.EntityId);
        }

        [Fact]
        public async Task Enqueue_LargeNumberOfMessages_AllAreReceived()
        {
            // Arrange
            var queue = new AuditQueue();
            const int count = 500;
            for (int i = 0; i < count; i++)
            {
                queue.Enqueue(new AuditMessage
                {
                    EntityId = $"id-{i}",
                    EntityType = i % 2 == 0 ? "Claim" : "Cover",
                    HttpRequestType = "POST"
                });
            }

            // Act & Assert
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            for (int i = 0; i < count; i++)
            {
                var m = await queue.Reader.ReadAsync(cts.Token);
                Assert.Equal($"id-{i}", m.EntityId);
            }
        }
    }
}