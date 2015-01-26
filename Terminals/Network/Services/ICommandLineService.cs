using System.ServiceModel;
using Terminals.CommandLine;

namespace Terminals.Network.Services
{
    [ServiceContract]
    public interface ICommandLineService
    {
        [OperationContract]
        void ForwardCommand(CommandLineArgs commandArguments);
    }
}