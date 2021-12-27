using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Botman.Commands
{
  public class BMGive : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Sends an item to a players inventory.";

    public override string[] GetCommands() => new[] { "bm-give" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-give (playername/steamid/entityid) (itemname) (count) (qual)\n" +
      "2. bm-give all (itemname) (count) (qual)\n" +
      "   1. Sends the specified item to a players inventory.\n " +
      "   2. Sends the specified item to all online players inventory\n" +
      "*If no quality is added and item has a quality, default is set to 1.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var silent = false;
      if (_params.Contains("/silent"))
      {
        silent = true;
        _params.Remove("/silent");
      }

      if (_params.Count > 4 || _params.Count < 3)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 3 or 4, found {_params.Count}");

        return;
      }

      if (_params[0].Length < 1 || _params[0].Length > 37)
      {
        SdtdConsole.Instance.Output($"Can not give item to \"{_params[0]}\": Invalid Playername, Steamid or entityid");

        return;
      }

      if (_params[1].Length < 1)
      {
        SdtdConsole.Instance.Output($"Invalid item \"{_params[1]}\"");

        return;
      }

      if (_params[2].Length < 1 || _params[2].Length > 5)
      {
        SdtdConsole.Instance.Output($"Invalid item count \"{_params[2]}\"");

        return;
      }

      var count = 1;
      if (int.TryParse(_params[2], out var _count))
      {
        if (_count > 0 && _count < 100000)
        {
          count = _count;
        }
      }

      var quality = 1;
      if (_params.Count == 4)
      {
        if (int.TryParse(_params[3], out var _quality))
        {
          if (_quality >= 1 && quality <= 6)
          {
            quality = _quality;
          }
          else
          {
            SdtdConsole.Instance.Output($"{_quality} is an invalid quality level. Must be a number 1-6.");

            return;
          }
        }
      }

      ItemClass itemClass;
      Block block;

      if (int.TryParse(_params[1], out var id))
      {
        itemClass = ItemClass.GetForId(id);
        block = Block.GetBlockByName(_params[1], true);
      }
      else
      {
        itemClass = ItemClass.GetItemClass(_params[1], true);
        block = Block.GetBlockByName(_params[1], true);
      }

      if (itemClass == null && block == null)
      {
        SdtdConsole.Instance.Output($"Unable to find item {_params[1]}");

        return;
      }

      ItemValue itemValue;
      try
      {
        itemValue = new ItemValue(ItemClass.GetItem(_params[1], true).type, false);
        if (itemValue.HasQuality)
        {
          itemValue.Quality = quality;
        }
      }
      catch (Exception e)
      {
        SdtdConsole.Instance.Output("Unable to get item: " + e);

        return;
      }

      if (_params[0].ToLower() == "all")
      {
        foreach (var cInfo in ConnectionManager.Instance.Clients.List.ToList())
        {
          if (cInfo == null) { continue; }

          GiveItem(cInfo, cInfo.playerName, itemValue, count, silent);
        }
      }
      else
      {
        var cInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);
        if (cInfo == null)
        {
          SdtdConsole.Instance.Output($"Player {_params[0]} does not exist");

          return;
        }

        GiveItem(cInfo, _params[0], itemValue, count, silent);
      }
    }

    private static void GiveItem(ClientInfo cInfo, string playerName, ItemValue itemValue, int count, bool silent)
    {
      var world = GameManager.Instance.World;
      if (world == null)
      {
        SdtdConsole.Instance.Output("World isn't loaded");

        return;
      }

      if (!world.Players.dict[cInfo.entityId].IsSpawned())
      {
        SdtdConsole.Instance.Output($"Player {playerName} is not spawned");

        return;
      }

      var entityItem = (EntityItem)EntityFactory.CreateEntity(
        new EntityCreationData
        {
          entityClass = EntityClass.FromString("item"),
          id = EntityFactory.nextEntityID++,
          itemStack = new ItemStack(itemValue, count),
          pos = world.Players.dict[cInfo.entityId].position,
          rot = new Vector3(20f, 0f, 20f),
          lifetime = 60f,
          belongsPlayerId = cInfo.entityId
        });

      world.SpawnEntityInWorld(entityItem);
      cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, cInfo.entityId));
      world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Killed);

      SdtdConsole.Instance.Output($"Gave {itemValue.ItemClass.GetLocalizedItemName() ?? itemValue.ItemClass.Name} to {playerName}.");

      if (!silent)
      {
        SendMessage.Private(cInfo, $"You were given {count} {itemValue.ItemClass.GetLocalizedItemName() ?? itemValue.ItemClass.Name}");
      }
    }
  }
}
