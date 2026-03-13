using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using Hive.SeedWorks.Invariants;
using Hive.SeedWorks.Result;

namespace Chat.Domain.Specifications
{
    /// <summary>
    /// Валидатор проверяющий, что сессия активна (не является начальной и не завершена).
    /// </summary>
    public class IsSessionActiveValidator : IBusinessOperationValidator<IChat, IChatAnemicModel>
    {
        public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj)
            => obj.Aggregate is not DefaultAnemicModel && obj.Aggregate.Root != null;

        public string Reason => "Сессия не активна или уже завершена.";

        public DomainOperationResultEnum DomainResult => DomainOperationResultEnum.Exception;
    }
}
