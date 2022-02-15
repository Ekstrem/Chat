using System;

namespace Chat.Api.Middlewares.Correlation
{
    public interface IExecutionContextAccessor
    {
        Guid CorrelationId { get; }
    }
}
