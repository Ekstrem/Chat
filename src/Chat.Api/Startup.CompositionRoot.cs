using Autofac;

namespace Chat.Api
{
    public partial class Startup
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<DomainServices.RegisterDependencies>();
            builder.RegisterModule<Application.RegisterDependencies>();
            builder.RegisterModule<Storage.RegisterDependencies>();
        }
    }
}
