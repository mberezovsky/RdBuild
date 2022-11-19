using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using RdBuild.Server;
using RdBuild.Shared.Enums;
using RdBuild.Shared.Protocol;
using RdBuild.Shared.SectionData;

namespace RdBuild.Coordinator;

public class CoordinatorCommandsProcessor : CommandProcessor<CommonEnums.CoordinatorCommands>
{

    public CoordinatorCommandsProcessor()
    {
        Get[CommonEnums.CoordinatorCommands.RegisterServerData] = RegisterServerData;
    }

    /// <summary>
    /// На входе в запросе следующие данные:
    ///     - ServerName
    ///     - List of
    ///         - JobName
    ///         - JobRunnerCount
    /// </summary>
    /// <param name="req"></param>
    /// <param name="resp"></param>
    /// <returns></returns>
    private Response.EResponseCode RegisterServerData(Request<CommonEnums.CoordinatorCommands> req, Response resp)
    {
        var requestReader = new RequestReader(req);
        requestReader.ReadObject<RegisterServerData>(out var regData);
        requestReader.ReadString(out var serverName);
        return Response.EResponseCode.OK;
    }
}

public class RegisterServerData   
{
}

internal class RequestReader
{
    Dictionary<EHeaderSectionType, Action<Stream>> m_readerActionList = new();
    public RequestReader() {}

    public void ReadObject<TObject>(out TObject res)
    {
        readerActionList.Add(();
    }
}