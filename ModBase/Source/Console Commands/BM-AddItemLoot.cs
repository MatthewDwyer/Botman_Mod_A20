using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMAddItemLoot : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Add item to Secure Loot below the given entity";

    public override string[] GetCommands() => new[] { "bm-additemloot" };

    public override string GetHelp() =>
      "Add item to Secure Loot Container below the given player\n" +
      "Usage:\n" +
      "1. bm-additemloot <name / entity id> <loot_slot_number> <item name> <stackSize> <quality> <used>\n" +
      "2. bm-additemloot <name / entity id> <item name> <stackSize> <quality> <usedTimes>\n" +
      "  1. add an item at SecureLoot at slot_number\n" +
      "  2. add an item at SecureLoot at first slot available\n" +
      "*if the item does not have quality, set the quality number as 1\n" +
      "*if the item does not have used, set the number as 0. This number is a % of used. 0% means new full item. 100 means totally broken\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 6 && _params.Count != 5)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 5 or 6, found {_params.Count}.");

        return;
      }

      if (!CmdHelpers.GetClientAndEntity(_params[0], out var clientInfo, out var entityPlayer)) { return; }

      var paramIndex = 1;
      var slotIndex = -1;

      if (_params.Count == 6)
      {
        if (!int.TryParse(_params[paramIndex], out slotIndex) || slotIndex < 0)
        {
          SdtdConsole.Instance.Output("loot_slot_number must be greater or equal to 0.");

          return;
        }

        paramIndex++;
      }

      var itemName = _params[paramIndex];

      paramIndex++;
      if (!int.TryParse(_params[paramIndex], out var stackSize))
      {
        SdtdConsole.Instance.Output("Stacksize must be a number.");

        return;
      }

      paramIndex++;
      if (!int.TryParse(_params[paramIndex], out var quality) || quality < 1)
      {
        SdtdConsole.Instance.Output("Quality must be greater or equal to 1.");

        return;
      }

      paramIndex++;
      if (!int.TryParse(_params[paramIndex], out var usedTimes) || usedTimes < 0 || usedTimes > 100)
      {
        SdtdConsole.Instance.Output("Used must be from 0 to 100.");

        return;
      }

      var itemValue = ItemClass.GetItem(itemName, true);

      if (itemValue.type == ItemValue.None.type)
      {
        SdtdConsole.Instance.Output("Item not found.");

        return;
      }

      itemValue = new ItemValue(itemValue.type, true);

      if (itemValue.HasQuality)
      {
        itemValue.Quality = quality;
      }

      if (itemValue.MaxUseTimes != 0)
      {
        itemValue.UseTimes = usedTimes == 0 ? 0 : itemValue.MaxUseTimes * usedTimes / 100;
      }

      if (itemValue.HasModSlots)
      {
        for (var j = 0; j < itemValue.Modifications.Length; j++)
        {
          itemValue.Modifications[j] = null;
        }
      }

      // todo: does this need a y-1 for the block below the player? or will reliance on the Math.floor be sufficient
      var tileEntity = GameManager.Instance.World.GetTileEntity(0, entityPlayer.GetBlockPosition());
      if (tileEntity == null)
      {
        SdtdConsole.Instance.Output($"additemLoot: fail, msg: block below player is not a secure loot, player: {entityPlayer.entityId}.");

        return;
      }

      var tileEntityType = tileEntity.GetTileEntityType();

      try
      {
        if (!tileEntityType.ToString().Equals("SecureLoot"))
        {
          SdtdConsole.Instance.Output($"additemLoot: fail, msg: block below player is not a secure loot, player: {entityPlayer.entityId}.");

          return;
        }

        var tileEntitySecureLootContainer = (TileEntitySecureLootContainer)tileEntity;

        if (!tileEntitySecureLootContainer.GetUsers().Contains(clientInfo.CrossplatformId) && !tileEntitySecureLootContainer.GetOwner().Equals(clientInfo.CrossplatformId))
        {
          SdtdConsole.Instance.Output($"additemLoot: fail, msg:forbidden, player: {entityPlayer.entityId} is not the owner and does not have access of the secure loot he is above.");

          return;
        }

        if (tileEntitySecureLootContainer.IsUserAccessing())
        {
          SdtdConsole.Instance.Output($"additemLoot: fail, msg:inUse, player: {entityPlayer.entityId}. Someone is accessing the secure loot.");

          return;
        }

        var itemStack = new ItemStack(itemValue, stackSize);
        if (slotIndex != -1)
        {
          if (slotIndex >= tileEntitySecureLootContainer.items.Length)
          {
            SdtdConsole.Instance.Output("Slot number bigger than loot container size.");

            return;
          }

          tileEntitySecureLootContainer.UpdateSlot(slotIndex, itemStack);
        }
        else if (!tileEntitySecureLootContainer.AddItem(itemStack))
        {
          SdtdConsole.Instance.Output("additemloot: fail. Container is full");

          return;
        }

        SdtdConsole.Instance.Output("additemloot: success. Item added");
      }
      catch (Exception ex)
      {
        Log.Out("~Botman Notice~ Error in bm-additemloot.execute: " + ex);
      }

      //var chunkClusters = GameManager.Instance.World.ChunkClusters;

      //for (var j = 0; j < chunkClusters.Count; j++)
      //{
      //  foreach (var chunk in chunkClusters[j].GetChunkArray())
      //  {
      //    foreach (var tileEntity in chunk.GetTileEntities().dict.Values)
      //    {
      //      var tileEntityType = tileEntity.GetTileEntityType();

      //      try
      //      {
      //        if (!tileEntityType.ToString().Equals("SecureLoot")) { continue; }

      //        var tileEntitySecureLootContainer = (TileEntitySecureLootContainer)tileEntity;

      //        var worldPos = tileEntitySecureLootContainer.ToWorldPos();

      //        if (worldPos.x != playerPosition.x || worldPos.z != playerPosition.z || worldPos.y != playerPosition.y) { continue; }

      //        if (!tileEntitySecureLootContainer.GetUsers().Contains(clientInfo.playerId) && !tileEntitySecureLootContainer.GetOwner().Equals(clientInfo.playerId))
      //        {
      //          SdtdConsole.Instance.Output($"additemLoot: fail, msg:forbidden, player: {entityPlayer.entityId} is not the owner and does not have access of the secure loot he is above.");

      //          return;
      //        }

      //        if (tileEntitySecureLootContainer.IsUserAccessing())
      //        {
      //          SdtdConsole.Instance.Output($"additemLoot: fail, msg:inUse, player: {entityPlayer.entityId}. Someone is accessing the secure loot.");

      //          return;
      //        }

      //        var itemStack = new ItemStack(itemValue, stackSize);
      //        if (slotIndex != -1)
      //        {
      //          if (slotIndex >= tileEntitySecureLootContainer.items.Length)
      //          {
      //            SdtdConsole.Instance.Output("Slot number bigger than loot container size.");

      //            return;
      //          }

      //          tileEntitySecureLootContainer.UpdateSlot(slotIndex, itemStack);
      //        }
      //        else if (!tileEntitySecureLootContainer.AddItem(itemStack))
      //        {
      //          SdtdConsole.Instance.Output("additemloot: fail. Container is full");

      //          return;
      //        }

      //        SdtdConsole.Instance.Output("additemloot: success. Item added");

      //        return;
      //      }
      //      catch (Exception ex)
      //      {
      //        Log.Out("~Botman Notice~ Error in bm-additemloot.execute: " + ex);
      //      }
      //    }
      //  }
      //}
    }
  }
}
