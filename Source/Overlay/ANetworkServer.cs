using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace TransportLayerOverlay
{
    /// <summary>
    /// Base network server class to inherit to apply a custom overlay
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ANetworkServer : MonoBehaviour, ITransportLayerHandlerProtocol
    {
        ///////////////////////////////
        ////////// Attribute //////////
        ///////////////////////////////
        
        ///////////////////////////////////
        ////////// Configuration //////////

        [SerializeField] protected int _MaxDefaultConnections = 10;
        [SerializeField] protected int _ServerPort = 2804;

        /////////////////////////////////
        ////////// Information //////////

        [SerializeField] protected int _ChannelId = -1;
        [SerializeField] protected List<int> _ConnectionIdRecords;
        [SerializeField] protected int _ServerHostId = -1;
        
        ////////////////////////////
        ////////// Method //////////
        ////////////////////////////

        /////////////////////////
        ////////// API //////////
        
        public void Send(string message)
        {
            var buffer = new byte[1024];
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(buffer);
            
            formatter.Serialize(stream, message);
            foreach (var connectionId in _ConnectionIdRecords)
            {
                byte networkErrorByteCode;
                
                NetworkTransport.Send(_ServerHostId, connectionId, _ChannelId, buffer, buffer.Length, out networkErrorByteCode);
                if ( ((NetworkError) networkErrorByteCode) != NetworkError.Ok )
                {
                    Debug.LogError(((NetworkError)networkErrorByteCode).ToString(), gameObject);
                }
            }
        }
        
        [ContextMenu("StartServer")]
        public void StartServer()
        {
            ConnectionConfig connectionConfiguration = new ConnectionConfig();

            _ChannelId = connectionConfiguration.AddChannel(QosType.Reliable);
            _ServerHostId = NetworkTransport.AddHost(new HostTopology(connectionConfiguration, _MaxDefaultConnections), _ServerPort);
            NetworkEventDispatcher.GetInstance.AddHost(_ServerHostId, this);
        }

        [ContextMenu("StopServer")]
        public void StopServer()
        {
            _ConnectionIdRecords.Clear();
            NetworkEventDispatcher.GetInstance.RemoveHost(_ServerHostId);
            NetworkTransport.RemoveHost(_ServerHostId);
            _ServerHostId = -1;
        }
        
        //////////////////////////////
        ////////// Callback //////////

        public virtual void OnConnection(int receivedHostId, int receivedConnectionId, int receivedChannelId)
        {
            _ConnectionIdRecords.Add(receivedConnectionId);
        }

        public virtual void OnDisconnection(int receivedHostId, int receivedConnectionId, int receivedChannelId)
        {
            _ConnectionIdRecords.Remove(receivedConnectionId);
        }

        public abstract void OnReceiveData(int receivedHostId, int receivedConnectionId, int receivedChannelId,
            byte[] buffer, int receivedDataSize);
    }
}
