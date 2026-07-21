using Mirror;
using UnityEngine;
using VContainer;

namespace NetworkMessages
{
    public class NetworkMessageManager : NetworkManager
    {
        private INetworkMessageService _messages;

        [Inject]
        public void Construct(INetworkMessageService messages)
        {
            _messages = messages;
        }

        public override void OnStartServer()
        {
            _messages.StartServer();

            _messages.ClientSubscribed += OnClientSubscribed;
        }

        public override void OnStopServer()
        {
            _messages.ClientSubscribed -= OnClientSubscribed;

            _messages.StopServer();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient connection)
        {
            _messages.RemoveConnection(connection);

            base.OnServerDisconnect(connection);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            _messages.Subscribe<HelloMessage>(OnHelloMessage);
        }

        private void OnClientSubscribed(NetworkConnectionToClient connection, ushort messageId)
        {
            if (messageId != Mirror.NetworkMessages.GetId<HelloMessage>())
            {
                return;
            }

            Debug.Log($"Server received client #{connection.connectionId} subscription to {nameof(HelloMessage)}.");

            _messages.TrySend(connection, new HelloMessage { Text = "Hello Client!" });
        }

        private void OnHelloMessage(HelloMessage message)
        {
            Debug.Log(message.Text);
        }
    }
}
