using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Definition;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Specifications;

/// <summary>Инвариант: изменять можно только существующее сообщение.</summary>
internal sealed class ExistingMessageValidator : IBusinessOperationValidator<IChat, IChatAnemicModel>
{
    public string Name => nameof(ExistingMessageValidator);

    public string Reason { get; private set; } = string.Empty;

    public DomainOperationResultEnum DomainResult { get; private set; } = DomainOperationResultEnum.Success;

    public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj)
    {
        var satisfied = obj.Aggregate.Text.Value.Length > 0;
        DomainResult = satisfied ? DomainOperationResultEnum.Success : DomainOperationResultEnum.Exception;
        Reason = satisfied ? string.Empty : "message: сообщение не найдено";
        return satisfied;
    }

    public bool ValidateModel(IAnemicModel<IChat> anemicModel)
        => anemicModel is IChatAnemicModel chat && chat.Text.Value.Length > 0;
}
