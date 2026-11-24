using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Characteristics;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Reactive;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>
/// Декоратор агрегата: проксирует бизнес-методы и рассылает их результат подписчикам
/// через <see cref="IObservable{T}"/>.
/// </summary>
internal sealed class Notifier :
    DomainEventNotifier<IChat, IChatAggregate>,
    IChatAggregate,
    IObservable<AggregateResult<IChat, IChatAnemicModel>>
{
    private readonly IChatAggregate _aggregate;
    private readonly List<IObserver<AggregateResult<IChat, IChatAnemicModel>>> _observers = new();

    private Notifier(IChatAggregate aggregate)
        : base(aggregate)
        => _aggregate = aggregate;

    public static Notifier Create(IChatAggregate aggregate) => new(aggregate);

    IDisposable IObservable<AggregateResult<IChat, IChatAnemicModel>>.Subscribe(
        IObserver<AggregateResult<IChat, IChatAnemicModel>> observer)
        => Unsubscriber<AggregateResult<IChat, IChatAnemicModel>>.Subscribe(_observers, observer);

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
        foreach (var observer in _observers)
        {
            observer.OnNext(result);
        }

        return result;
    }
}
