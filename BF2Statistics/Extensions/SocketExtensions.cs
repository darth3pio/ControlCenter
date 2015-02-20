namespace System.Net.Sockets
{
    public static class SocketExtensions
    {
        /// <summary>
        /// Determines if the connection to the machine on the other side of the socket is still active.
        /// Serves the same purpose as TcpClient.Connected and TcpListener.Connected.
        /// </summary>
        /// <param name="iSocket"></param>
        /// <returns>A bool of whether the remote client is still connected.</returns>
        public static bool IsConnected(this Socket iSocket)
        {
            try
            {
                return !(iSocket.Poll(1, SelectMode.SelectRead) && iSocket.Available == 0);
            }
            catch
            {
                return false;
            }
        }
    }
}
