using NUnit.Framework;
using RdBuild.Client;
using RdBuild.Shared.Protocol;

namespace RdBuild.Shared.Tests
{
    public class CommandWorkingTests
    {
        [Test]
        public void NoOpCommandTest()
        {
            var cmd = new Command<CommonCommands>(CommonCommands.Noop);
        }

        
    }
}