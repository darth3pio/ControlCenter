using BF2Statistics.Gamespy;

namespace BF2Statistics
{
    public delegate void ShutdownEventHandler();

    public delegate void StartupEventHandler();

    public delegate void ConnectionUpdate(object sender);

    public delegate void AspRequest();

    public delegate void SnapshotProccessed();

    public delegate void SnapshotRecieved();

    public delegate void DataRecivedEvent(string Message);

    public delegate void ConnectionClosed();

    public delegate void GpspConnectionClosed(GpspClient client);

    public delegate void GpcmConnectionClosed(GpcmClient client);

    public delegate void MstrConnectionClosed(MasterClient client);

    public delegate void ServerChangedEvent();
}
