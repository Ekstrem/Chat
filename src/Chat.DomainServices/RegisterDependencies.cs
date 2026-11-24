using Autofac;
using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>Модуль Autofac: регистрация доменных сервисов контекста Chat.</summary>
public sealed class RegisterDependencies : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // QG-105 требует в одном модуле и провайдера, и наблюдателя результата агрегата.
        var Provider = typeof(AggregateProvider);
        var IObserver = typeof(IObserver<AggregateResult<IChat, IChatAnemicModel>>);

        builder
            .RegisterType(Provider)
            .As<IAggregateProviderWithCompletion<IChat, IChatAggregate>>()
            .As<IAggregateProvider<IChat, IChatAggregate>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<BusAdapter>()
            .As(IObserver)
            .InstancePerLifetimeScope();
    }
}
