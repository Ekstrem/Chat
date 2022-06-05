using Autofac;
using Autofac.Features.Indexed;
using Chat.InternalContracts;
using Hive.SeedWorks.Monads;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Chat.MaterializedView
{
    public class RegisterDependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterDomainServices(builder);

        private static ContainerBuilder RegisterDomainServices(ContainerBuilder builder)
            => builder
                .RegisterMediatR(typeof(RegisterDependencies).Assembly)
                .Do(b => b
                    .RegisterType<ViewDbContext>()
                    .Keyed<DbContext>(nameof(MaterializedView)))
                .Do(b => b
                    .Register(c => c
                        .Resolve<IIndex<string, DbContext>>()
                        .PipeTo(cs => cs[nameof(MaterializedView)])
                        .PipeTo(ctx => new QueryRepository<DialogView>(ctx)))
                    .As<IQueryRepository<DialogView>>());
    }
}
