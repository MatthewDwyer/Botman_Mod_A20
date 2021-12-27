using System.Collections.Generic;

namespace Botman.Commands
{
  class BMRemoveQuest : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman-Removes a player a quest";

    public override string[] GetCommands() => new[] { "bm-removequest" };

    public override string GetHelp() =>
      "Usage:\n" +
      " 1. bm-removequest playername/entityid  QuestName\n" +
      "   1. Removes a target players quest\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 2)
      {
        SdtdConsole.Instance.Output(GetHelp());

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

      clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup($"removequest {questId}", true));

      SdtdConsole.Instance.Output($"Removed {questId} from {clientInfo.playerName}");
    }
  }
}
