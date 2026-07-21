using VContainer;
using VContainer.Unity;

namespace NetworkMessages
{
    public class NetworkMessagesLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<NetworkMessageService>(Lifetime.Singleton).As<INetworkMessageService>();

            builder.RegisterComponentInHierarchy<NetworkMessageManager>();
        }
    }
}
