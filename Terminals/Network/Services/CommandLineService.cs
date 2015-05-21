using System.ServiceModel;
using Terminals.CommandLine;

namespace Terminals.Network.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CommandLineService : ICommandLineService
    {
        private readonly MainForm mainForm;

        public CommandLineService(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void ForwardCommand(CommandLineArgs args)
        {
            this.mainForm.HandleCommandLineActions(args);
            this.mainForm.BringToFront();
            this.mainForm.Focus();
        }
    }
}