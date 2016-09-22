using System;
using System.ServiceModel;
using Kohl.Framework.Info;

namespace Terminals.Network.Services
{
    public class CommandLineServer : ServiceHost
    {
        private static readonly string BASE_ADDRESS = "net.pipe://localhost/" + AssemblyInfo.Title +
                                                      "/CommandLineService";

        public CommandLineServer(MainForm mainForm)
            : base(new CommandLineService(mainForm), new Uri(BASE_ADDRESS))

        {
            this.AddServiceEndpoint(typeof(ICommandLineService), new NetNamedPipeBinding(), BASE_ADDRESS);
        }

        public static ICommandLineService CreateClient()
        {
            ChannelFactory<ICommandLineService> factory =
                new ChannelFactory<ICommandLineService>(new NetNamedPipeBinding(),
                                                        new EndpointAddress(BASE_ADDRESS));
            return factory.CreateChannel();
        }
    }
}