using System;
using System.Collections.Concurrent;
using RdBuild.Shared.Protocol;

namespace RdBuild.Server;

public interface ICommandProcessor<TECommandType> where TECommandType : Enum
{
    Response.EResponseCode ProcessRequest(Request<TECommandType> request, Response response);
}
public interface ICommandProcessorBuilder<TECommandType> where TECommandType: Enum
{
    void RegisterRoute(TECommandType commandType, Func<Request<TECommandType>, Response, Response.EResponseCode> processFunction,
        Predicate<Request<TECommandType>> filterFunction = null);
}

public class CommandProcessor<TECommandType> : 
    ICommandProcessor<TECommandType>, 
    ICommandProcessorBuilder<TECommandType> where TECommandType: Enum
{

    class RouteItem
    {
        public  Predicate<Request<TECommandType>>? Filter { get; set; }
        public Func<Request<TECommandType>, Response, Response.EResponseCode> Action { get; set; }
    }

    readonly ConcurrentDictionary<TECommandType, List<RouteItem>> m_routes = new();

    protected CommandProcessor()
    {
        Get = new CommandRoutes<TECommandType>(this);
    }

    public void RegisterRoute(
        TECommandType command,
        Func<Request<TECommandType>, Response, Response.EResponseCode> processFunction,
        Predicate<Request<TECommandType>>? filterFunction = null)
    {
        var route = new RouteItem { Action = processFunction, Filter = filterFunction };
        m_routes.AddOrUpdate(command, cmd => new List<RouteItem>() { route }
            , (cmd, routeItems) =>
            {
                routeItems.Add(route);
                return routeItems;
            });
    }

    public Response.EResponseCode ProcessRequest(Request<TECommandType> request, Response response)
    {
        response.Code = Response.EResponseCode.CommandNotFound;

        if (m_routes.TryGetValue(request.Command, out var routeList))
        {
            foreach (var route in routeList)
            {
                if (route.Filter != null)
                    if (!route.Filter.Invoke(request))
                        continue;

                var res = route.Action.Invoke(request, response);
                response.Code = res;
                return response.Code;
            }
        }

        return response.Code;
    }

    protected CommandRoutes<TECommandType> Get;

    public class CommandReader
    {
        private Request<TECommandType> Read(Stream iStream)
        {
            throw new NotImplementedException();
        }
    }
}


public class CommandRoutes<TCommandEnum> where TCommandEnum : Enum
{
    private Dictionary<TCommandEnum, Action<Request<TCommandEnum>, Response>> requestsActions = new();
    private readonly ICommandProcessorBuilder<TCommandEnum> m_owner;

    public CommandRoutes(ICommandProcessorBuilder<TCommandEnum> owner)
    {
        m_owner = owner;
    }

    public Func<Request<TCommandEnum>, Response, Response.EResponseCode> this[TCommandEnum idx]
    {
        set => m_owner.RegisterRoute(idx, value);
    }
}