using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Result;

namespace Chat.DomainServices;

/// <summary>Р Р°СЃС€РёСЂРµРЅРёСЏ РґРµРєРѕСЂРёСЂРѕРІР°РЅРёСЏ Р°РіСЂРµРіР°С‚Р° РЅР°Р±Р»СЋРґР°С‚РµР»СЏРјРё.</summary>
internal static class NotifierExtension
{
    /// <summary>РћР±РµСЂРЅСѓС‚СЊ Р°РіСЂРµРіР°С‚ РІ <see cref="Notifier"/> СЃ СѓРєР°Р·Р°РЅРЅС‹РјРё РЅР°Р±Р»СЋРґР°С‚РµР»СЏРјРё.</summary>
    public static IChatAggregate PipeNotifierToContract(
        this IChatAggregate aggregate,
        IEnumerable<IObserver<AggregateResult<IChat, IChatAnemicModel>>> observers)
        => Notifier.Create(aggregate, observers);
}
