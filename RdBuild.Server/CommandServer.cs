using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RdBuild.Server
{
    /// <summary>
    /// Сервер для сведения вместе и демона, который слушает сеть, и CommandProcessor-а, который обрабатывает
    /// пакеты из сети и посылает их в сеть.
    /// Надо подумать, как организовать работу с несколькими EndPoint-ами.
    /// Скорее всего ServerDaemon должен выдавать сигналы на подключение новых клиентов,
    /// а данный сервер - принимать решение на то, разрешать их подключение или нет и на запуск обработчика
    /// подключений.
    /// </summary>
    public class CommandServer<TCommandEnum> where TCommandEnum : Enum
    {
        private ServerDaemon? m_daemon;
        private readonly int m_port;
        private readonly CommandProcessor<TCommandEnum> m_commandProcessor;

        public CommandServer(int serverPort, CommandProcessor<TCommandEnum> commandProcessor)
        {
            m_port = serverPort;
            m_commandProcessor = commandProcessor;
            m_daemon = null;
        }

        public void Start(CancellationToken token)
        {
            m_daemon = new ServerDaemon(m_port, ClientConnectedAction);
        }

        private void ClientConnectedAction(Socket socket)
        {
            // Здесь мы должны начать обработку запросов от входящего клиента...
            // то есть, базовый CommandProcessor подключаем сюда, он принимает и парсит запросы
            // а дочерний процессор - их обрабатывает путем регистрации своих путей в его GET-запросном реестре.
            throw new NotImplementedException();
        }
    }

}
