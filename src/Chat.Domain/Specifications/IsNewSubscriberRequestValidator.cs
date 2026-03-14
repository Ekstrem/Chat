using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.Domain.Specifications
{
    /// <summary>
    /// Валидатор проверяющий, что сущность новая.
    /// </summary>
    public class IsNewSubscriberRequestValidator : IBusinessOperationValidator<IChat, IChatAnemicModel>
    {
        public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj)
            => obj.Aggregate is DefaultAnemicModel;

        public string Reason => "Операция применима только для новых сущностей.";

        public DomainOperationResultEnum DomainResult => DomainOperationResultEnum.Exception;
    }
}
