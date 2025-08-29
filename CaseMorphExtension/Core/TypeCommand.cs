using System.Threading;
using Microsoft.CommandPalette.Extensions.Toolkit;

using WindowsInput;

#nullable enable
namespace CaseMorphExtension.Core
{
    public partial class TypeCommand : InvokableCommand
    {
        private readonly string _data;
        private const int Delay = 200;

        public TypeCommand(string data)
        {
            _data = data;
            Name = "Type";
            Icon = new IconInfo("\uE765");
        }

        private void Type()
        {
            Thread.Sleep(Delay);
            InputSimulator sim = new();
            sim.Keyboard.TextEntry(_data);
        }

        public override CommandResult Invoke()
        {
            var thread = new Thread(Type);
            thread.Start();
            return CommandResult.Hide();
        }
    }
}
