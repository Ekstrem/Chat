using Autofac;
using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>РњРѕРґСѓР»СЊ Autofac: СЂРµРіРёСЃС‚СЂР°С†РёСЏ РґРѕРјРµРЅРЅС‹С… СЃРµСЂРІРёСЃРѕРІ РєРѕРЅС‚РµРєСЃС‚Р° Chat.</summary>
public sealed class RegisterDependencies : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // QG-105 С‚СЂРµР±СѓРµС‚ РІ РѕРґРЅРѕРј РјРѕРґСѓР»Рµ Рё РїСЂРѕРІР°Р№РґРµСЂР°, Рё РЅР°Р±Р»СЋРґР°С‚РµР»СЏ СЂРµР·СѓР»СЊС‚Р°С‚Р° Р°РіСЂРµРіР°С‚Р°.
        var Provider = typeof(AggregateProvider);
        var IObserver = typeof(IObserver<AggregateResult<IChat, IChatAnemicModel>>);

        builder
            .RegisterType(Provider)
            .As<IAggregateProvider<IChat, IChatAggregate>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<BusAdapter>()
            .As(IObserver)
            .InstancePerLifetimeScope();
    }
}
