namespace TransportLayerOverlay
{
    /// <summary>
    /// Interface used to standardize event processing
    /// </summary>
    public interface ITransportLayerHandlerProtocol
    {
        /// <summary>
        /// Call when the host connect
        /// </summary>
        void OnConnection(int receivedHostId, int receivedConnectionId, int receivedChannelId);
        
        /// <summary>
        /// Call when the host disconnect
        /// </summary>
        void OnDisconnection(int receivedHostId, int receivedConnectionId, int receivedChannelId);
        
        /// <summary>
        /// Call when the host receives data
        /// </summary>
        void OnReceiveData(int receivedHostId, int receivedConnectionId, int receivedChannelId, byte[] buffer, int receivedDataSize);
    }
}
