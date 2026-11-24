using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.DomainServices;

/// <summary>Расширения декорирования агрегата наблюдателями.</summary>
internal static class NotifierExtension
{
    /// <summary>Обернуть агрегат в <see cref="Notifier"/> и подписать наблюдателей.</summary>
    public static Notifier PipeNotifierToContract(
        this IChatAggregate aggregate,
        IEnumerable<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
    {
        var notifier = Notifier.Create(aggregate);
        foreach (var observer in observers)
        {
            ((IObservable<AggregateResult<IChat, IChatAnemicModel>>)notifier).Subscribe(observer);
        }

        return notifier;
    }
}
