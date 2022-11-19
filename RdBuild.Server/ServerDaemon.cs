using System.Net;
using System.Net.Sockets;

namespace RdBuild.Server
{
    internal class ServerDaemon
    {
        private readonly int m_port;
        private readonly Action<Socket> m_clientConnectedAction;

        public ServerDaemon(int port, Action<Socket> clientConnectedAction)
        {
            m_port = port;
            m_clientConnectedAction = clientConnectedAction;
        }

        public void Run(CancellationToken token)
        {
            Task.Run(() => DoRun(token), token);

        }

        private void DoRun(CancellationToken token)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, m_port);
            Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            using (token.Register(() => listener.Close()))
            {
                listener.Bind(endpoint);
                listener.Listen(m_port);

                while (!token.IsCancellationRequested)
                {
                    Socket clientSocket = listener.Accept();
                    if (token.IsCancellationRequested)
                        break;
                    if (m_clientConnectedAction != null)
                        m_clientConnectedAction.Invoke(clientSocket);
                }
            }
        }

    }
}