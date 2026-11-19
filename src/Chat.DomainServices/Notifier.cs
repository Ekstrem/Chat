using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Characteristics;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>
/// Декоратор агрегата: после каждой успешной бизнес-операции уведомляет наблюдателей
/// (шину событий) результатом операции.
/// </summary>
internal sealed class Notifier :
    DomainEventNotifier<IChat, IChatAggregate>,
    IChatAggregate
{
    private readonly IChatAggregate _aggregate;
    private readonly IReadOnlyList<IObserver<AggregateResult<IChat, IChatAnemicModel>>> _observers;

    private Notifier(
        IChatAggregate aggregate,
        IReadOnlyList<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
        : base(aggregate)
    {
        _aggregate = aggregate;
        _observers = observers;
    }

    public static IChatAggregate Create(
        IChatAggregate aggregate,
        IEnumerable<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
        => new Notifier(aggregate, observers.ToList());

    public Guid Id => _aggregate.Id;

    public long Version => _aggregate.Version;

    public string CommandName => _aggregate.CommandName;

    public string SubjectName => _aggregate.SubjectName;

    public Guid CorrelationToken => _aggregate.CorrelationToken;

    public IDictionary<string, IValueObject> Invariants => _aggregate.Invariants;

    public IDictionary<string, IValueObject> GetValueObjects() => _aggregate.GetValueObjects();

    public AggregateResult<IChat, IChatAnemicModel> SendMessage(
        ICommandToAggregate command,
        string product,
        string role,
        string to,
        string text,
        IEnumerable<Guid>? attachments,
        Guid? replyTo,
        IReadOnlyDictionary<string, string>? extendedFields)
        => Notify(_aggregate.SendMessage(command, product, role, to, text, attachments, replyTo, extendedFields));

    public AggregateResult<IChat, IChatAnemicModel> AddAttachment(ICommandToAggregate command, Guid attachmentId)
        => Notify(_aggregate.AddAttachment(command, attachmentId));

    public AggregateResult<IChat, IChatAnemicModel> ReadMessage(ICommandToAggregate command, string addressee)
        => Notify(_aggregate.ReadMessage(command, addressee));

    private AggregateResult<IChat, IChatAnemicModel> Notify(AggregateResult<IChat, IChatAnemicModel> result)
    {
        if (result.Result == DomainOperationResultEnum.Success)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(result);
            }
        }

        return result;
    }
}
