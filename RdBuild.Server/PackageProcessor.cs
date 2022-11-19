using System.Net.Sockets;
using RdBuild.Shared.Protocol;

namespace RdBuild.Server;

/// <summary>
/// Processor for working between the ServerDaemon and CommandProcessor. Must do the following:
/// 1. Receive the packages from the connection.
/// 2. Store information about all registered CommandProcessors
/// 3. Dispatch received requests to appropriate CommandProcessor.
/// </summary>
class PackageProcessor
{
    public void ProcessConnection(Socket clientSocket)
    {

        IRequest
    }


}