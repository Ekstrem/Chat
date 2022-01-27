using Chat.Domain.Abstraction;
using Hive.SeedWorks.Invariants;

namespace Chat.Domain.Implementation
{
    internal sealed class BusinessOperationData : BusinessOperationData<IChat, IChatAnemicModel>
    {
        private BusinessOperationData(IChatAnemicModel aggregate, IChatAnemicModel model)
            : base(aggregate, model)
        {
        }

        public static BusinessOperationData Commit(IChatAnemicModel aggregate, IChatAnemicModel model)
            => new BusinessOperationData(aggregate, model);
    }
}
