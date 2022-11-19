using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;

namespace RdBuild.Shared.Tests.Server.Tests
{
    public class ServerTests
    {
        [Test]
        public void ServerStartupTest()
        {
            int connectionsCount = 0;
            var daemon = new RdBuild.Server.ServerDaemon(12345, socket => connectionsCount++);
            var tokenSource = new CancellationTokenSource();
            daemon.Run(tokenSource.Token);
            Assert.That(connectionsCount, Is.EqualTo(0));
            DoConnection(100);
            Assert.That(connectionsCount, Is.EqualTo(1));
            DoConnection(100);
            Assert.That(connectionsCount, Is.EqualTo(2));
            tokenSource.Cancel();
        }

        private static void DoConnection(int timeout)
        {
            var client = new TcpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            client.Connect(endPoint);
            Thread.Sleep(timeout);
            client.Close();
        }
    }
}