using Autofac;
using Chat.Infrastructure.Messaging.Kafka;
using Hive.SeedWorks.Monads;

namespace Chat.Infrastructure
{
    public class RegisterDependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterDomainServices(builder);

        private static ContainerBuilder RegisterDomainServices(ContainerBuilder builder)
            => builder
                .Do(b => b
                    .RegisterType<KafkaProducer>()
                    .As<IExternalEventProducer>())
                .Do(b => b
                    .RegisterType<KafkaConsumerFactory<string, string>>()
                    .As<IKafkaConsumerFactory<string, string>>()
                    .InstancePerLifetimeScope());
    }
}
