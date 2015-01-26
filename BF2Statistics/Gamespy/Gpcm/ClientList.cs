using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Gamespy
{
    class ClientList : EventArgs
    {
        public List<GpcmClient> Clients;

        public ClientList(List<GpcmClient> Clients)
        {
            this.Clients = Clients;
        }
    }
}
