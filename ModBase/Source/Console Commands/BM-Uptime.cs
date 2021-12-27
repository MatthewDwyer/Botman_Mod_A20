using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMUptime : BMCmdAbstract
  {
    public static MicroStopwatch UpTime = new MicroStopwatch();

    public override string GetDescription() => "~Botman~ Display the running duration for the server";

    public override string[] GetCommands() => new[] { "bm-uptime" };

    public override string GetHelp() =>
      "Display the running duration for the server\n" +
      "Usage:\n" +
      "1. bm-uptime\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 0)
      {
        SdtdConsole.Instance.Output(GetHelp());
        SdtdConsole.Instance.Output("Command does not take any arguments.");

        return;
      }

      var time = TimeSpan.FromMilliseconds(UpTime.ElapsedMilliseconds);
      SdtdConsole.Instance.Output($"{time.Hours:D2}h:{time.Minutes:D2}m:{time.Seconds:D2}s");
    }
  }
}
