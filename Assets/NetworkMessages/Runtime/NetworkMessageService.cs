using System;
using System.Collections.Generic;
using Mirror;

namespace NetworkMessages
{
    public class NetworkMessageService : INetworkMessageService
    {
        private readonly Dictionary<int, HashSet<ushort>> _subscriptions = new();

        public event Action<NetworkConnectionToClient, ushort> ClientSubscribed;

        public void StartServer()
        {
            NetworkServer.ReplaceHandler<NetworkMessageSubscription>(OnSubscription);
        }

        public void StopServer()
        {
            NetworkServer.UnregisterHandler<NetworkMessageSubscription>();

            _subscriptions.Clear();
        }

        public void RemoveConnection(NetworkConnectionToClient connection)
        {
            _subscriptions.Remove(connection.connectionId);
        }

        public void Subscribe<T>(Action<T> handler) where T : struct, NetworkMessage
        {
            if (!NetworkClient.isConnected)
            {
                throw new InvalidOperationException("Mirror client is not connected.");
            }

            NetworkClient.ReplaceHandler<T>(handler);
            NetworkClient.Send(new NetworkMessageSubscription { MessageId = Mirror.NetworkMessages.GetId<T>() });
        }

        public bool TrySend<T>(NetworkConnectionToClient connection, T message) where T : struct, NetworkMessage
        {
            var messageId = Mirror.NetworkMessages.GetId<T>();
            var hasSubscriptions = _subscriptions.TryGetValue(connection.connectionId, out var messageIds);

            if (!hasSubscriptions || !messageIds.Contains(messageId))
            {
                return false;
            }

            connection.Send(message);

            return true;
        }

        private void OnSubscription(NetworkConnectionToClient connection, NetworkMessageSubscription message)
        {
            if (!_subscriptions.TryGetValue(connection.connectionId, out var messageIds))
            {
                messageIds = new HashSet<ushort>();

                _subscriptions.Add(connection.connectionId, messageIds);
            }

            if (messageIds.Add(message.MessageId))
            {
                ClientSubscribed?.Invoke(connection, message.MessageId);
            }
        }
    }
}
