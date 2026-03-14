using System;
using System.Threading.Tasks;
using Chat.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Chat.Application.Tests.Behaviors
{
    public class ExceptionHandlingBehaviorTests
    {
        [Fact]
        public async Task ExceptionHandlingBehavior_OnException_ReturnsFailure()
        {
            var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<TestCommand, ChatOperationResult>>>();
            var behavior = new ExceptionHandlingBehavior<TestCommand, ChatOperationResult>(loggerMock.Object);

            RequestHandlerDelegate<ChatOperationResult> next =
                () => throw new InvalidOperationException("test error");

            var result = await behavior.Handle(
                new TestCommand(),
                next,
                default);

            Assert.False(result.IsSuccess);
            Assert.Contains("test error", result.Reason);
        }

        [Fact]
        public async Task ExceptionHandlingBehavior_NoException_ReturnsResult()
        {
            var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<TestCommand, ChatOperationResult>>>();
            var behavior = new ExceptionHandlingBehavior<TestCommand, ChatOperationResult>(loggerMock.Object);

            var expected = ChatOperationResult.Success(Guid.NewGuid(), 1, "OK");
            RequestHandlerDelegate<ChatOperationResult> next = () => Task.FromResult(expected);

            var result = await behavior.Handle(
                new TestCommand(),
                next,
                default);

            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result);
        }

        public class TestCommand : IRequest<ChatOperationResult> { }
    }
}
