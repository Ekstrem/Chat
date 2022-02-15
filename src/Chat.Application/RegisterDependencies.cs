using Autofac;
using AutoMapper;
using Chat.Application.Mappers;
using Hive.SeedWorks.Monads;
using MediatR.Extensions.Autofac.DependencyInjection;

namespace Chat.Application
{
    public class RegisterDependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterApplicationLayer(builder);

        private static ContainerBuilder RegisterApplicationLayer(ContainerBuilder builder)
        {
            builder
                .RegisterMediatR(typeof(RegisterDependencies).Assembly)
                .Do(b => b
                    .RegisterInstance(InitAutoMapper()));

            return builder;
        }

        public static IMapper InitAutoMapper()
            => new MapperConfiguration(x => x.AddProfile<AggregateProfile>())
                .PipeTo(config => config.CreateMapper());
    }
}
