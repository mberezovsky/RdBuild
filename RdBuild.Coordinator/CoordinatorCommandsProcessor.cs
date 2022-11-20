using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
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
        var requestReader = new RequestReader();
        requestReader.ReadObject<RegisterServerData>(out var regData);
        requestReader.ReadParameter(CommonEnums.EParameterNames.ServerName, out string serverName);
        return Response.EResponseCode.OK;
    }
}

public class RegisterServerData   
{
}

internal class RequestReader
{
    readonly Dictionary<EHeaderSectionType, Action<BinaryReader>> m_readerActionList = new();
    public RequestReader() {}

    public void ReadObject<TObject>(out TObject res)
    {
        m_readerActionList.Add(EHeaderSectionType.ObjectSection, reader =>
        {
            string buffer = reader.ReadString();
            JsonConvert.DeserializeObject<TObject>(buffer);
        });
    }

    public void ReadParameter(CommonEnums.EParameterNames serverName, out string s)
    {
        throw new NotImplementedException();
    }
}