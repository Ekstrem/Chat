using System;
using System.Linq;
using Xunit;

namespace Chat.Application.Tests
{
    public class ChatOperationResultTests
    {
        [Fact]
        public void Success_SetsProperties()
        {
            var id = Guid.NewGuid();
            var version = 42L;
            var resultText = "Created";

            var result = ChatOperationResult.Success(id, version, resultText);

            Assert.Equal(id, result.AggregateId);
            Assert.Equal(version, result.Version);
            Assert.Equal(resultText, result.Result);
        }

        [Fact]
        public void Failure_SetsProperties()
        {
            var reasons = new[] { "Error1", "Error2" };

            var result = ChatOperationResult.Failure(reasons);

            Assert.Equal("Error", result.Result);
            Assert.Equal(reasons, result.Reason);
        }

        [Fact]
        public void Success_IsSuccessTrue()
        {
            var result = ChatOperationResult.Success(Guid.NewGuid(), 1, "OK");

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Failure_IsSuccessFalse()
        {
            var result = ChatOperationResult.Failure("something went wrong");

            Assert.False(result.IsSuccess);
        }
    }
}
