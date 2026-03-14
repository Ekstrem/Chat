using System;
using Autofac;
using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Monads;
using DigiTFactory.Libraries.SeedWorks.Result;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public class RegisterDependencies: Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterDomainServices(builder);

        private static ContainerBuilder RegisterDomainServices(ContainerBuilder builder)
            => builder
                .Do(b => b
                    .RegisterType<AggregateProvider>()
                    .As<IAggregateProvider<IChat, IChatAnemicModel>>()
                    .As<IChatAggregateProvider>())
                .Do(b => b
                    .RegisterType<BusAdapter>()
                    .As<IObserver<AggregateResult<IChat, IChatAnemicModel>>>());
    }
}
