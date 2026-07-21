using Mirror;

namespace NetworkMessages
{
    public struct HelloMessage : NetworkMessage
    {
        public string Text;
    }

    public struct NetworkMessageSubscription : NetworkMessage
    {
        public ushort MessageId;
    }
}
