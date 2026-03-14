using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Invariants;

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
