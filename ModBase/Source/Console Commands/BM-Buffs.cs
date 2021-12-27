using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMBuffs : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Returns a players buffs";

    public override string[] GetCommands() => new[] { "bm-buffs" };

    public override string GetHelp() =>
      "Usage:\n" +
      "  1. bm-buffs playername/steamid/entityid \n" +
      "  2. bm-buffs all \n" +
      "  3. bm-buffs filter {tag} \n" +
      "    1. Returns active buffs of a player online \n" +
      "    2. Returns active buffs of all players online \n" +
      "    3. Returns all online players with tag filter entered\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].Equals("filter", StringComparison.InvariantCultureIgnoreCase))
      {
        foreach (var cInfo in ConnectionManager.Instance.Clients.List)
        {
          if (cInfo == null) { continue; }

          var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
          if (entityPlayer == null) { continue; }

          var buffs = entityPlayer.Buffs.ActiveBuffs;
          if (buffs == null) { continue; }

          foreach (var buff in buffs)
          {
            if (buff.BuffName.ToLower().Contains(_params[1].ToLower()))
            {
              SdtdConsole.Instance.Output($"  - {cInfo.playerName} has {buff.BuffName}");
            }
          }
        }

        return;
      }

      if (_params[0].Equals("all", StringComparison.InvariantCultureIgnoreCase))
      {
        foreach (var cInfo in ConnectionManager.Instance.Clients.List)
        {
          if (cInfo == null) { continue; }

          var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
          if (entityPlayer != null)
          {
            PrintPlayerBuffs(cInfo, entityPlayer);
          }
        }

        return;
      }

      var cInfo2 = ConsoleHelper.ParseParamIdOrName(_params[0], true);
      if (cInfo2 != null)
      {
        PrintPlayerBuffs(cInfo2, GameManager.Instance.World.Players.dict[cInfo2.entityId]);

        return;
      }

      SdtdConsole.Instance.Output($"Could not locate {_params[0]}");
    }

    public static void PrintPlayerBuffs(ClientInfo _cInfo, EntityPlayer player)
    {
      var buffs = player.Buffs.ActiveBuffs;
      if (buffs == null) { return; }

      SdtdConsole.Instance.Output(string.Empty);
      SdtdConsole.Instance.Output($"Active Buffs for {_cInfo.playerName}");

      foreach (var buff in buffs)
      {
        if (!buff.BuffClass.Hidden && !string.IsNullOrWhiteSpace(buff.BuffName))
        {
          SdtdConsole.Instance.Output($"  - {buff.BuffName}");
        }
      }
    }
  }
}
