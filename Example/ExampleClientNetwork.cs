using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace TransportLayerOverlay.Example
{
    /// <inheritdoc/>
    /// <summary>
    /// Example implement of ANetworkClient
    /// </summary>
    public sealed class ExampleClientNetwork : ANetworkClient
    {
        ////////////////////////////
        ////////// Method //////////
        ////////////////////////////
        
        /////////////////////////
        ////////// API //////////
        
        [ContextMenu("SendHelloWorldToServer")]
        public void SendHelloWorldToServer()
        {
            Send("Hello World");
        }
        
        //////////////////////////////
        ////////// Callback //////////
        
        public override void OnConnection(int receivedHostId, int receivedConnectionId, int receivedChannelId)
        {
            Debug.Log("Client connection: Host:" + receivedHostId.ToString() + " ConnectionId:" + receivedConnectionId.ToString());
        }

        public override void OnDisconnection(int receivedHostId, int receivedConnectionId, int receivedChannelId)
        {
            Debug.Log("Client disconnection: Host:" + receivedHostId.ToString() + " ConnectionId:" + receivedConnectionId.ToString());
        }

        public override void OnReceiveData(int receivedHostId, int receivedConnectionId, int receivedChannelId,
            byte[] buffer, int receivedDataSize)
        {
            var stream = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();
            var message = formatter.Deserialize(stream) as string;
            
            Debug.Log("Client received data: " + message);
        }
    }
}
