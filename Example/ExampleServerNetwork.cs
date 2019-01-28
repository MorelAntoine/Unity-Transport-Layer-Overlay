using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace TransportLayerOverlay.Example
{
    /// <inheritdoc/>
    /// <summary>
    /// Basic example of an implement of the ANetworkServer
    /// </summary>
    public sealed class ExampleServerNetwork : ANetworkServer
    {
        ////////////////////////////
        ////////// Method //////////
        ////////////////////////////
        
        /////////////////////////
        ////////// API //////////
        
        [ContextMenu("SendServerMasterHelloToAllClient")]
        public void SendServerMasterHelloToAllClient()
        {
            Send("ServerMasterHello");
        }
        
        //////////////////////////////
        ////////// Callback //////////
        
        public override void OnConnection(int receivedHostId, int receivedConnectionId, int receivedChannelId)
        {
            base.OnConnection(receivedHostId, receivedConnectionId, receivedChannelId);
            Debug.Log("Server connection: Host:" + receivedHostId.ToString() + " ConnectionId:" + receivedConnectionId.ToString());
        }

        public override void OnDisconnection(int receivedHostId, int receivedConnectionId, int receivedChannelId)
        {
            base.OnDisconnection(receivedHostId, receivedConnectionId, receivedChannelId);
            Debug.Log("Server disconnection: Host:" + receivedHostId.ToString() + " ConnectionId:" + receivedConnectionId.ToString());
        }

        public override void OnReceiveData(int receivedHostId, int receivedConnectionId, int receivedChannelId,
            byte[] buffer, int receivedDataSize)
        {
            var stream = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();
            var message = formatter.Deserialize(stream) as string;
            
            Debug.Log("Server received data: " + message);
        }
    }
}
