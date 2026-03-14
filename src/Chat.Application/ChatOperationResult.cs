using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Application
{
    public class ChatOperationResult
    {
        public Guid AggregateId { get; private set; }
        public long Version { get; private set; }
        public string Result { get; private set; } = string.Empty;
        public IEnumerable<string> Reason { get; private set; } = Enumerable.Empty<string>();
        public bool IsSuccess { get; private set; }

        public static ChatOperationResult Success(Guid id, long version, string result)
            => new() { AggregateId = id, Version = version, Result = result, IsSuccess = true, Reason = Enumerable.Empty<string>() };

        public static ChatOperationResult Failure(params string[] reasons)
            => new() { IsSuccess = false, Reason = reasons, Result = "Error" };
    }
}
