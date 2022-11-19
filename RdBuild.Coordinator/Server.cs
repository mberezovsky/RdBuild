using System;
using System.Threading;
using RdBuild.Server;
using RdBuild.Shared.Enums;
using RdBuild.Shared.Protocol;

namespace RdBuild.Coordinator
{
    /// <summary>
    /// Координатор должен работать следующим образом.
    /// Он принимает регистрацию от серверов и запросы на использование ресурсов от клиентов.
    /// Возможно, потом будет реализована система взаимодействия нескольких координаторов.
    /// 
    /// Сервер, регистрируясь на координаторе, передает следующее:
    ///     - имя серевера, порт, на который он принимает обращения
    ///     - пары {JobType, RunnersCount}, которые он готов обрабатывать.
    /// На выходе он получает свой serverId, через который он потом будет общаться с координатором.
    ///
    /// Координатор обязан проверять жизнеспособность сервера через ping-пакеты RUT
    /// То есть, сервер должен содержать обработчики сразу нескольких команд
    /// </summary>
    public class CoordinatorServer
    {
        private readonly CoordinatorConfiguration m_configuration;
        private CommandServer<CommonEnums.ESystemCommands> m_systemCommandServer;
        private readonly CommandServer<CommonEnums.CoordinatorCommands> m_coordinatorServer;

        public CoordinatorServer(CoordinatorConfiguration configuration)
        {
            m_configuration = configuration;
            m_systemCommandServer = new CommandServer<CommonEnums.ESystemCommands>(configuration.SystemCommandPort, 
                new SystemCommandProcessor());
            m_coordinatorServer =
                new CommandServer<CommonEnums.CoordinatorCommands>(configuration.CoordinatorCommandsPort, 
                    new CoordinatorCommandsProcessor());
        }

        public void Start(CancellationToken token)
        {
            m_systemCommandServer.Start(token);
            m_coordinatorServer.Start(token);
        }
    }

    class SystemCommandProcessor : CommandProcessor<CommonEnums.ESystemCommands>
    {
        public SystemCommandProcessor()
        {
            Get[CommonEnums.ESystemCommands.RUT] = ProcessRutRequest;
        }

        private Response.EResponseCode ProcessRutRequest(Request<CommonEnums.ESystemCommands> request, Response response)
        {
            return Response.EResponseCode.OK;
        }
    }
}