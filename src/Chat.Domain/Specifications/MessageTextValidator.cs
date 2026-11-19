using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Definition;
using DigiTFactory.Libraries.SeedWorks.Invariants;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Specifications;

/// <summary>
/// Инварианты текста: не пустой, ≤ 32 КБ UTF-8, без BOM и без символов-заменителей.
/// </summary>
internal sealed class MessageTextValidator : IBusinessOperationValidator<IChat, IChatAnemicModel>
{
    public string Name => nameof(MessageTextValidator);

    public string Reason { get; private set; } = string.Empty;

    public DomainOperationResultEnum DomainResult { get; private set; } = DomainOperationResultEnum.Success;

    public bool IsSatisfiedBy(BusinessOperationData<IChat, IChatAnemicModel> obj)
    {
        var satisfied = MessageText.TryCreate(obj.Model.Text.Value, out _, out var reason);
        DomainResult = satisfied ? DomainOperationResultEnum.Success : DomainOperationResultEnum.Exception;
        Reason = reason;
        return satisfied;
    }

    public bool ValidateModel(IAnemicModel<IChat> anemicModel)
        => anemicModel is IChatAnemicModel chat
           && MessageText.TryCreate(chat.Text.Value, out _, out _);
}
