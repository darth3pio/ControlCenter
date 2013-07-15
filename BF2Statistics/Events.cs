using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics
{
    public delegate void ShutdownEventHandler();

    public delegate void StartupEventHandler();

    public delegate void ConnectionUpdate(object sender);

    public delegate void AspRequest();

    public delegate void SnapshotProccessed();

    public delegate void SnapshotRecieved(bool Proccessed);
}
