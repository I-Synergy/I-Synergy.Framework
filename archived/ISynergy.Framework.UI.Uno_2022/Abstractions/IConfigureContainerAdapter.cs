using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.UI.Abstractions;

internal interface IConfigureContainerAdapter
{
    void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder);
}
