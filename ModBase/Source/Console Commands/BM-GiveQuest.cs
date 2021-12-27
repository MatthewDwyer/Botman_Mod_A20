using System.Collections.Generic;

namespace Botman.Commands
{
  class BMGivequest : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman-Gives a player a quest";

    public override string[] GetCommands() => new[] { "bm-givequest" };

    public override string GetHelp() =>
      "Usage:\n" +
      " 1. bm-givequest <playername/entityid> <QuestName>\n" +
      " 2. bm-givequest list\n" +
      "   1. Gives a target player a quest\n" +
      "   2. List QuestName\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0 ||
          _params.Count == 1 && !_params[0].Equals("list"))
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      // _params[0] must be "list" to pass the above if
      if (_params.Count == 1)
      {
        //todo: split lines if this is a long list
        SdtdConsole.Instance.Output(string.Join(", ", QuestClass.s_Quests.Keys));

        return;
      }

      if (!CmdHelpers.GetClientInfo(_params[0], out var clientInfo)) { return; }

      // Note: s_Quests is a case insensitive dictionary
      var questId = QuestClass.s_Quests[_params[1]]?.ID;
      if (questId == null)
      {
        SdtdConsole.Instance.Output($"Quest '{_params[1]}' does not exist!");

        return;
      }

      clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup($"givequest {questId}", true));

      SdtdConsole.Instance.Output($"Gave {questId} to {clientInfo.playerName}");
    }
  }
}
