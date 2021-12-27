using System;
using System.Collections.Generic;
using System.Linq;

namespace Botman.Commands
{
  public class BMCheckLooter : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Check items from SecureLoot below the given entity";

    public override string[] GetCommands() => new[] { "bm-checkloot" };

    public override string GetHelp() =>
      "Check items from SecureLoot below the given entity\n" +
      "Usage:\n" +
      "1. bm-checkloot <name / entity id> \n" +
      "   1. check items of Secure Loot under a player\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 found {_params.Count}.");

        return;
      }

      if (!CmdHelpers.GetEntityPlayer(_params[0], out var entityPlayer)) { return; }

      var playerPosition = entityPlayer.GetBlockPosition();

      //todo: get chunk player is in and then get tileEntity at position instead of looping all loaded chunks?
      for (var i = 0; i < GameManager.Instance.World.ChunkClusters.Count; i++)
      {
        foreach (var chunk in GameManager.Instance.World.ChunkClusters[i].GetChunkArray())
        {
          foreach (var tileEntity in chunk.GetTileEntities().dict.Values)
          {
            var tileEntityType = tileEntity.GetTileEntityType();
            try
            {
              if (!tileEntityType.ToString().Equals("SecureLoot")) { continue; }

              var secureLootContainer = (TileEntitySecureLootContainer)tileEntity;

              if (secureLootContainer.ToWorldPos() != playerPosition) { continue; }

              var num = 0;

              SdtdConsole.Instance.Output($"Items: {secureLootContainer.items.Length}");

              foreach (var itemStack in secureLootContainer.items)
              {
                if (itemStack.IsEmpty()) { continue; }

                SdtdConsole.Instance.Output(
                  $"checkLoot: {num++}, " +
                  $"player={entityPlayer.entityId}, " +
                  $"item={ItemClass.list[itemStack.itemValue.type].GetItemName()}, " +
                  $"qnty={itemStack.count}, " +
                  $"quality={itemStack.itemValue.Quality}, " +
                  $"Mods=({Mods(itemStack.itemValue)}), " +
                  $"used={(int)itemStack.itemValue.UseTimes * 100 / Math.Max(1, itemStack.itemValue.MaxUseTimes)}");
              }

              return;
            }
            catch (Exception e)
            {
              Log.Out("~Botman Notice~ Error in bm-checkloot.execute: " + e);
            }
          }
        }
      }
    }

    private static string Mods(ItemValue itemValue) =>
      itemValue.HasModSlots && itemValue.Modifications != null && itemValue.Modifications.Length > 0
        ? string.Join(", ", itemValue.Modifications.Where(m => !m.IsEmpty()))
        : string.Empty;
  }
}
