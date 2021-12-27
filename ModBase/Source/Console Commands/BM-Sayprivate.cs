using System.Collections.Generic;
using System.Linq;

namespace Botman.Commands
{
  public class BMSayPrivate : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Custom bot public chat responses";

    public override string[] GetCommands() => new[] { "bm-sayprivate" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1.  bm-sayprivate (playername,steamid,entityid) Hello World \n" +
      "* If the players name has spaces, use quotation marks around name.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!CmdHelpers.GetClientInfo(_params[0], out var clientInfo)) { return; }

      var message = string.Join(" ", _params.Skip(1));

      SendMessage.Private(clientInfo, message);

      SdtdConsole.Instance.Output($"Sent to {clientInfo.playerName}: {message}");
    }
  }
}
