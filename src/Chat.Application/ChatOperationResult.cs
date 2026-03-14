using System;
using System.Collections.Generic;

namespace Chat.Application
{
    /// <summary>
    /// DTO результата операции над агрегатом чата.
    /// </summary>
    public class ChatOperationResult
    {
        public Guid AggregateId { get; init; }
        public long Version { get; init; }
        public string Result { get; init; }
        public IEnumerable<string> Reason { get; init; }
        public bool IsSuccess { get; init; }
    }
}
