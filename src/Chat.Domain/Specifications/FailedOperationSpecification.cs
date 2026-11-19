using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.Domain.Specifications;

/// <summary>
/// Спецификация «операция отклонена»: несёт причину отказа в <see cref="AggregateResultException{TBoundedContext,TModel}"/>.
/// </summary>
internal sealed class FailedOperationSpecification : IBusinessOperationSpecification<IChat, IChatAnemicModel>
{
    public FailedOperationSpecification(string reason) => Reason = reason;

    public string Reason { get; }

    public DomainOperationResultEnum DomainResult => DomainOperationResultEnum.Exception;

    public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj) => false;
}
