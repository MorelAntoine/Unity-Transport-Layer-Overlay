using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TransportLayerOverlay
{
  /// <inheritdoc/>
  /// <summary>
  /// Dispatch all the network event receive to the correct host
  /// </summary>
  [DisallowMultipleComponent]
  public sealed class NetworkEventDispatcher : MonoBehaviour
  {
    ///////////////////////////////
    ////////// Attribute //////////
    ///////////////////////////////

    /////////////////////////////////
    ////////// Information //////////
    
    private Dictionary<int, ITransportLayerHandlerProtocol> _hostRecords = null;
    
    ///////////////////////////////
    ////////// Singleton //////////

    private static NetworkEventDispatcher _instance = null;
    
    //////////////////////////////
    ////////// Property //////////
    //////////////////////////////

    public static NetworkEventDispatcher GetInstance => _instance;
    
    ////////////////////////////
    ////////// Method //////////
    ////////////////////////////
    
    /////////////////////////////////////
    ////////// Host Management //////////

    public void AddHost(int hostId, ITransportLayerHandlerProtocol transportLayerHandler)
    {
      _hostRecords.Add(hostId, transportLayerHandler);
    }

    public void RemoveHost(int hostId)
    {
      _hostRecords.Remove(hostId);
    }
    
    ////////////////////////////////////////////
    ////////// MonoBehaviour Callback //////////

    private void Awake()
    {
      if (_instance == null)
      {
        _instance = this;
        InitializeNetworkTransportLayer();
        _hostRecords = new Dictionary<int, ITransportLayerHandlerProtocol>();
      }
      else
      {
        Destroy(this);
      }
    }
    
    private void Update()
    {
      if ( _hostRecords.Count > 0 )
      {
        HandleNetworkEvent();
      }
    }
    
    /////////////////////////////
    ////////// Service //////////

    private void HandleNetworkEvent()
    {
      var buffer = new byte[1024];
      var lastNetworkEventType = NetworkEventType.DataEvent;
          
      while ( lastNetworkEventType != NetworkEventType.Nothing )
      {
        byte networkErrorByteCode;
        int receivedChannelId;
        int receivedConnectionId;
        int receivedDataSize;
        int receivedHostId;
              
        lastNetworkEventType = NetworkTransport.Receive(out receivedHostId, out receivedConnectionId,
          out receivedChannelId, buffer, buffer.Length, out receivedDataSize, out networkErrorByteCode);
        if ( ((NetworkError) networkErrorByteCode) != NetworkError.Ok )
        {
          Debug.Log(((NetworkError)networkErrorByteCode).ToString());
          return;
        }
        if ( !_hostRecords.ContainsKey(receivedHostId) )
        {
          Debug.Log($"Unknown receivedHostId: " + receivedHostId.ToString());
          return;
        }
        switch (lastNetworkEventType)
        {
          case NetworkEventType.ConnectEvent:
            _hostRecords[receivedHostId].OnConnection(receivedHostId, receivedConnectionId, receivedChannelId);
            break;
          case NetworkEventType.DataEvent:
            _hostRecords[receivedHostId].OnReceiveData(receivedHostId, receivedConnectionId, receivedChannelId, buffer, receivedDataSize);
            break;
          case NetworkEventType.DisconnectEvent:
            _hostRecords[receivedHostId].OnDisconnection(receivedHostId, receivedConnectionId, receivedChannelId);
            break;
        }
      }
    }
    
    private void InitializeNetworkTransportLayer()
    {
      NetworkTransport.Init();
    }
    
  }
}
