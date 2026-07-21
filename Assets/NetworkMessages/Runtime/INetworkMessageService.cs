using System;
using Mirror;

namespace NetworkMessages
{
    public interface INetworkMessageService
    {
        event Action<NetworkConnectionToClient, ushort> ClientSubscribed;

        void StartServer();
        void StopServer();
        void RemoveConnection(NetworkConnectionToClient connection);

        void Subscribe<T>(Action<T> handler) where T : struct, NetworkMessage;
        bool TrySend<T>(NetworkConnectionToClient connection, T message) where T : struct, NetworkMessage;
    }
}
