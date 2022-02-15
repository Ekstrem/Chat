using Autofac;
using Chat.DomainServices;
using Hive.SeedWorks.Monads;

namespace Chat.Storage
{
    public class RegisterDependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterDomainServices(builder);

        private static ContainerBuilder RegisterDomainServices(ContainerBuilder builder)
        {
            builder
                .Do(b => b
                    .RegisterType<Repository>()
                    .As<IRepository>());

            return builder;
        }
    }
}
