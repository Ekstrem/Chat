using Autofac;
using Autofac.Features.Indexed;
using Chat.DomainServices;
using Hive.SeedWorks.Monads;
using Chat.InternalContracts;
using Microsoft.EntityFrameworkCore;
using MediatR.Extensions.Autofac.DependencyInjection;

namespace Chat.Storage
{
    public class RegisterDependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterDomainServices(builder);

        private static ContainerBuilder RegisterDomainServices(ContainerBuilder builder)
            => builder
                .RegisterMediatR(typeof(RegisterDependencies).Assembly)
                .Do(b => b
                    .RegisterType<CommandDbContext>()
                    .Keyed<DbContext>(nameof(Storage)))
                .Do(b => b
                    .RegisterType<Repository>()
                    .As<IRepository>())
                .Do(b => b
                    .Register(c => c
                        .Resolve<IIndex<string, DbContext>>()
                        .PipeTo(cs => cs[nameof(Storage)])
                        .PipeTo(ctx => new QueryRepository<DomainEventEntry>(ctx)))
                    .As<IQueryRepository<DomainEventEntry>>());
    }
}
