using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Definition;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Specifications;

/// <summary>Инвариант адресата: <c>@роль</c>, несколько через «+» или <c>ALL</c>.</summary>
internal sealed class AddresseeValidator : IBusinessOperationValidator<IChat, IChatAnemicModel>
{
    public string Name => nameof(AddresseeValidator);

    public string Reason { get; private set; } = string.Empty;

    public DomainOperationResultEnum DomainResult { get; private set; } = DomainOperationResultEnum.Success;

    public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj)
    {
        var to = string.Join(" + ", obj.Model.Addressee.To);
        var satisfied = MessageAddressee.TryCreate(to, out _, out var reason);
        DomainResult = satisfied ? DomainOperationResultEnum.Success : DomainOperationResultEnum.Exception;
        Reason = reason;
        return satisfied;
    }

    public bool ValidateModel(IAnemicModel<IChat> anemicModel)
        => anemicModel is IChatAnemicModel chat
           && MessageAddressee.TryCreate(string.Join(" + ", chat.Addressee.To), out _, out _);
}
