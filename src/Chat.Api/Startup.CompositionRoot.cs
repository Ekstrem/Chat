using Autofac;
using Microsoft.EntityFrameworkCore;
using Hive.SeedWorks.Monads;
using Microsoft.Extensions.Configuration;
using Chat.Api.BackgroundServices;
using Chat.Api.BackgroundServices.EventBusConsumer;

namespace Chat.Api
{
    public partial class Startup
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder
                .RegisterModule<Infrastructure.RegisterDependencies>()
                .RegisterModule<DomainServices.RegisterDependencies>()
                .RegisterModule<Application.RegisterDependencies>()
                .RegisterModule<Storage.RegisterDependencies>();

            builder
                .Register(c => c
                    .Resolve<IConfiguration>()
                    .PipeTo(config => config["DbConnectionStrings"])
                    .PipeTo(cs => new DbContextOptionsBuilder<DbContext>()
                        .UseSqlServer(cs)
                        .Options))
                .As<DbContextOptions<DbContext>>();

            builder
                .RegisterType<EventMessageFactory>()
                .SingleInstance();

            builder
                .RegisterType<EventBusMessageHandler>()
                .SingleInstance();

            builder
                .RegisterType<EventBusConsumer>()
                .As<IEventBusConsumer>();
        }
    }
}
