using System.Collections.Generic;
using System.Threading;

namespace Botman.Commands
{
  public class BMClearInventory : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Kicks a player and clears their entire inventory.";

    public override string[] GetCommands() => new[] { "bm-clearinventory", "bm-clrinv" };

    public override string GetHelp() =>
      "Kicks a player and clears their entire inventory.\n" +
      "Usage:\n" +
      "1. bm-clearinventory <name/steamid> \n" +
      "   1. Will kick a player, allowing their inventory to be wiped.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

            string id = PersistentContainer.Instance.Players.GetId(_params[0]);
      if (id == null)
      {
        SdtdConsole.Instance.Output($"~Botman~ Could not locate player {_params[0]}");

        return;
      }

      var wipe = new InventoryWipe();
      wipe.Execute(id);

      SdtdConsole.Instance.Output($"~Botman~ Inventory wipe complete on {PersistentContainer.Instance.Players[id, true].PlayerName}/{id} ");
    }
  }

  internal class InventoryWipe
  {
    public string SteamId;

    public void Execute(string steamId)
    {
      SteamId = steamId;

      var thread = new Thread(Run);
      thread.Start();
    }

    public void Run()
    {
      WipeInventory(SteamId);
    }

    public void WipeInventory(string steamId)
    {
      var playerDataFile = new PlayerDataFile();
      playerDataFile.Load(GameIO.GetPlayerDataDir(), steamId);

      var cInfo = ConsoleHelper.ParseParamIdOrName(steamId, true, true);
      if (cInfo != null)
      {
        var iConsole = new AsyncCommand();
        SdtdConsole.Instance.ExecuteAsync($"kick {steamId} \"[ff0000]Your inventory has been wiped! You are free to log back in whenever you like.[-]\"", iConsole);
        Thread.Sleep(2000);
      }

      foreach (var slot in playerDataFile.inventory)
      {
        slot.Clear();
      }

      //todo: does this need to be saved each time, or just do it at the end?
      playerDataFile.Save(GameIO.GetPlayerDataDir(), steamId);

      foreach (var slot in playerDataFile.bag)
      {
        slot.Clear();
      }

      playerDataFile.Save(GameIO.GetPlayerDataDir(), steamId);

      for (var i = 0; i < playerDataFile.equipment.GetSlotCount(); i++)
      {
        var slotItem = playerDataFile.equipment.GetSlotItem(i);
        if (!slotItem.IsEmpty())
        {
          slotItem.Clear();
        }
      }

      playerDataFile.Save(GameIO.GetPlayerDataDir(), steamId);
    }
  }
}
