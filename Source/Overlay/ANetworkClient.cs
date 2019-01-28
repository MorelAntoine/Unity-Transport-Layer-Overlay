using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace TransportLayerOverlay
{
    /// <summary>
    /// Base network client class to inherit to apply a custom overlay
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ANetworkClient : MonoBehaviour, ITransportLayerHandlerProtocol
    {
        ///////////////////////////////
        ////////// Attribute //////////
        ///////////////////////////////
        
        ///////////////////////////////////
        ////////// Configuration //////////

        [SerializeField] protected string _ServerIPAddress = "127.0.0.1";
        [SerializeField] protected int _ServerPort = 2804;

        /////////////////////////////////
        ////////// Information //////////

        [SerializeField] protected int _ChannelId = -1;
        [SerializeField] protected int _ClientHostId = -1;
        [SerializeField] protected int _ConnectionId = -1;
        
        ////////////////////////////
        ////////// Method //////////
        ////////////////////////////

        /////////////////////////
        ////////// API //////////

        [ContextMenu("AttemptToConnectToServer")]
        public void AttemptToConnectToServer()
        {
            byte networkErrorByteCode;

            _ConnectionId = NetworkTransport.Connect(_ClientHostId, _ServerIPAddress, _ServerPort, 0, out networkErrorByteCode);
            if ( ((NetworkError) networkErrorByteCode) != NetworkError.Ok )
            {
                Debug.LogError(((NetworkError)networkErrorByteCode).ToString(), gameObject);
            }
        }

        public void Send(string message)
        {
            var buffer = new byte[1024];
            var formatter = new BinaryFormatter();
            byte networkErrorByteCode;
            var stream = new MemoryStream(buffer);
            
            formatter.Serialize(stream, message);
            NetworkTransport.Send(_ClientHostId, _ConnectionId, _ChannelId, buffer, buffer.Length, out networkErrorByteCode);
            if ( ((NetworkError) networkErrorByteCode) != NetworkError.Ok )
            {
                Debug.LogError(((NetworkError)networkErrorByteCode).ToString(), gameObject);
            }
        }
        
        [ContextMenu("StartClient")]
        public void StartClient()
        {
            ConnectionConfig connectionConfiguration = new ConnectionConfig();

            _ChannelId = connectionConfiguration.AddChannel(QosType.Reliable);
            _ClientHostId = NetworkTransport.AddHost(new HostTopology(connectionConfiguration, 1), 0);
            NetworkEventDispatcher.GetInstance.AddHost(_ClientHostId, this);
        }

        [ContextMenu("StopClient")]
        public void StopClient()
        {
            byte networkErrorByteCode;
            
            NetworkTransport.Disconnect(_ClientHostId, _ConnectionId, out networkErrorByteCode);
            NetworkEventDispatcher.GetInstance.RemoveHost(_ClientHostId);
            NetworkTransport.RemoveHost(_ClientHostId);
            _ClientHostId = -1;
        }
        
        //////////////////////////////
        ////////// Callback //////////

        public abstract void OnConnection(int receivedHostId, int receivedConnectionId, int receivedChannelId);

        public abstract void OnDisconnection(int receivedHostId, int receivedConnectionId, int receivedChannelId);

        public abstract void OnReceiveData(int receivedHostId, int receivedConnectionId, int receivedChannelId,
            byte[] buffer, int receivedDataSize);
    }
}
