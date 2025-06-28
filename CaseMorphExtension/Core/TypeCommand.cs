using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaseMorphExtension.Core
{
    public partial class TypeCommand : InvokableCommand
    {
        private readonly string _data;

        public TypeCommand(string data)
        {
            _data = data;
            Name = "Type";
            Icon = new IconInfo("\ue764");
            return;
        }

        public override CommandResult Invoke()
        {
            var _ = Task.Run(() =>
            {
                // Temporary workaround: Send Alt+Tab to switch to the target application   
                SendKeys.SendWait("%{TAB}");

                Thread.Sleep(300);

                SendKeys.SendWait(_data);
            });

            return CommandResult.Dismiss();
        }
    }
}