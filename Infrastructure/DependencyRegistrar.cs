using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.GittiGidiyor.Services;

namespace Nop.Plugin.Misc.GittiGidiyor.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {
            builder.RegisterType<GittiGidiyorManager>().AsSelf().InstancePerLifetimeScope();
        }

        public int Order => 2;
    }
}