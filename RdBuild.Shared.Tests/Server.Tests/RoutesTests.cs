
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using NUnit.Framework;
using RdBuild.Server;
using RdBuild.Shared.Protocol;

public class RoutesTests
{

    public enum MyCommands
    {
        Command1,
        Command2,
        Command3,
        Command4,
    }

    public enum MyModules
    {
        Module1
    }

    public class MyCommandProcessor : CommandProcessor<MyModules, MyCommands>
    {
        public MyCommandProcessor() : base(MyModules.Module1)
        {
            Get[MyCommands.Command1] = (request1, response) => Response.EResponseCode.OK;
            Get[MyCommands.Command2] = (request, response) => Response.EResponseCode.InternalError;
            Get[MyCommands.Command3] = (request, response) => response.SetText("Hello");
        }

    }

    [Test]
    public void RoutesTest1()
    {
        var processor = new MyCommandProcessor();
        var request = new Request<MyCommands>() {Command = MyCommands.Command1};
        var response = new Response();

        var res = processor.ProcessRequest(request, response);
        Assert.That(res, Is.EqualTo(Response.EResponseCode.OK));

        request.Command = MyCommands.Command2;
        res = processor.ProcessRequest(request, response);
        Assert.That(res, Is.EqualTo(Response.EResponseCode.InternalError));

        request.Command = MyCommands.Command3;
        res = processor.ProcessRequest(request, response);
        Assert.That(res, Is.EqualTo(Response.EResponseCode.OK));
    }

}