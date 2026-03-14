using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.Domain.Specifications
{
    /// <summary>
    /// Валидатор проверяющий, что обращение ещё не взято оператором.
    /// </summary>
    public class IsNotAlreadyDequeuedValidator : IBusinessOperationValidator<IChat, IChatAnemicModel>
    {
        public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj)
            => obj.Aggregate.Actor == null || obj.Aggregate.Actor.Type != UserType.Operator;

        public string Reason => "Обращение уже взято оператором на обработку.";

        public DomainOperationResultEnum DomainResult => DomainOperationResultEnum.Exception;
    }
}
