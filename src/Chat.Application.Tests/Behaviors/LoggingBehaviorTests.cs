using System.Threading.Tasks;
using Chat.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Chat.Application.Tests.Behaviors
{
    public class LoggingBehaviorTests
    {
        [Fact]
        public async Task LoggingBehavior_CallsNext()
        {
            var loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
            var behavior = new LoggingBehavior<TestRequest, TestResponse>(loggerMock.Object);

            var expectedResponse = new TestResponse { Value = "OK" };
            RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);

            var result = await behavior.Handle(
                new TestRequest(),
                next,
                default);

            Assert.Equal(expectedResponse, result);
            Assert.Equal("OK", result.Value);
        }

        public class TestRequest : IRequest<TestResponse> { }
        public class TestResponse { public string Value { get; set; } }
    }
}
