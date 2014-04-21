using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Gamespy;

namespace BF2Statistics
{
    public delegate void ShutdownEventHandler();

    public delegate void StartupEventHandler();

    public delegate void ConnectionUpdate(object sender);

    public delegate void AspRequest();

    public delegate void SnapshotProccessed();

    public delegate void SnapshotRecieved(bool Proccessed);

    public delegate void DataRecivedEvent(string Message);

    public delegate void ConnectionClosed();

    public delegate void GpspConnectionClosed(GpspClient client);

    public delegate void GpcmConnectionClosed(GpcmClient client);
}
