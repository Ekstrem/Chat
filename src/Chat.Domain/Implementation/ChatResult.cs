using Chat.Domain.Abstraction;
using Hive.SeedWorks.Invariants;
using Hive.SeedWorks.Result;
using System.Collections.Generic;

namespace Chat.Domain.Implementation
{
    internal class ChatResult : AggregateResult<IChat, IChatAnemicModel>
    {
        private readonly DomainOperationResultEnum _result;
        private readonly IEnumerable<string> _reason;

        internal ChatResult(
            IChatAnemicModel oldVersion,
            IChatAnemicModel newVersion,
            DomainOperationResultEnum result,
            IEnumerable<string> reason)
            : base(MakeBusinessOperationData(oldVersion, newVersion))
        {
            _result = result;
            _reason = reason;
        }

        private static BusinessOperationData<IChat, IChatAnemicModel> MakeBusinessOperationData(
            IChatAnemicModel oldVersion, IChatAnemicModel newVersion)
            => BusinessOperationData<IChat, IChatAnemicModel>
                .Commit<IChat, IChatAnemicModel>(oldVersion, newVersion);

        public override DomainOperationResultEnum Result => _result;

        public override IEnumerable<string> Reason => _reason;
    }
}
