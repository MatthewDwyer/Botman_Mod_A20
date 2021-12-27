using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMRemoveItem : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Remove items from SecureLoot below the given entity";

    public override string[] GetCommands() => new[] { "bm-remitem", "bm-removeitem" };

    public override string GetHelp() =>
      "Removes all items from Secure Loot Container below the given player\n" +
      "Usage:\n" +
      "1. bm-remitem <name / entity id>\n" +
      "   1. Removes all items from Secure Loot Container under player\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count < 1)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 or more, found {_params.Count}.");

        return;
      }

      var clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
      if (clientInfo == null)
      {
        SdtdConsole.Instance.Output("Playername or entity id not found.");

        return;
      }

      var entityPlayer = GameManager.Instance.World.Players.dict[clientInfo.entityId];
      if (entityPlayer == null)
      {
        SdtdConsole.Instance.Output("Unable to get player`s position");

        return;
      }

      var pos = entityPlayer.GetBlockPosition();
      var x = pos.x;
      var y = pos.y;
      var z = pos.z;

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

              var tileEntitySecureLootContainer = (TileEntitySecureLootContainer)tileEntity;

              var tilePosition = tileEntitySecureLootContainer.ToWorldPos();
              if (tilePosition.x != x || tilePosition.z != z || tilePosition.y != y) { continue; }

              var playerId = clientInfo.CrossplatformId;
              if (!tileEntitySecureLootContainer.GetUsers().Contains(playerId) && !tileEntitySecureLootContainer.GetOwner().Equals(playerId))
              {
                SdtdConsole.Instance.Output($"lootRemover: fail, msg:forbidden, player: {entityPlayer.entityId} is not the owner and does not have access of the secure loot he is above.");

                return;
              }

              if (tileEntitySecureLootContainer.IsUserAccessing())
              {
                SdtdConsole.Instance.Output("lootRemover: fail, msg:inUse, player: " + entityPlayer.entityId + ". Someone is accessing the secure loot.");

                return;
              }

              var items = tileEntitySecureLootContainer.items;

              SdtdConsole.Instance.Output($"Items: {items.Length}");

              var index = 0;
              var slotIndex = 0;
              foreach (var itemStack in items)
              {
                if (!itemStack.IsEmpty())
                {
                  index++;

                  SdtdConsole.Instance.Output(
                    $"lootRemover: {index}, " +
                    $"player={entityPlayer.entityId}, " +
                    $"item={ItemClass.list[itemStack.itemValue.type].GetItemName()}, " +
                    $"qnty={itemStack.count}, " +
                    $"quality={itemStack.itemValue.Quality}, " +
                    $"parts=({Mods(itemStack.itemValue)}), " +
                    $"used={(int)itemStack.itemValue.UseTimes * 100 / Math.Max(1, itemStack.itemValue.MaxUseTimes)}");

                  tileEntitySecureLootContainer.UpdateSlot(slotIndex, ItemStack.Empty);
                }
                slotIndex++;
              }
              return;
            }
            catch (Exception arg)
            {
              Log.Out("Error in RemoveFromLooter.Run: " + arg);
            }
          }
        }
      }
    }

    private static string Mods(ItemValue itemValue)
    {
      if (!itemValue.HasModSlots)
      {
        return "()";
      }

      var mods = itemValue.Modifications;
      if (mods == null || mods.Length == 0)
      {
        return "()";
      }

      var result = "";

      for (var j = 0; j < mods.Length; j++)
      {
        if (mods[j].IsEmpty()) { continue; }

        result += $"{mods[j].ItemClass.GetLocalizedItemName()}{(j == mods.Length ? "" : ", ")}";
      }

      return $"({result})";
    }
  }
}
