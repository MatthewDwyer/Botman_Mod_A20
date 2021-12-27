using System.Collections.Generic;
using UnityEngine;

namespace Botman.Commands
{
  public class BMGiveAt : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Give an item to a location";

    public override string[] GetCommands() => new[] { "bm-giveat" };

    public override string GetHelp() =>
      "Give an item to a a location \n" +
      "Usage:\n" +
      "1. bm-giveat <x> <y> <z> <item name> <amount>\n" +
      "2. bm-giveat <x> <y> <z> <item name> <amount> <quality>\n" +
      "Item name has to be the exact name of an item as listed by \"listitems\".\n" +
      "Amount is the number of instances of this item to drop (as a single stack).\n" +
      "Quality is the quality of the dropped items for items that have a quality.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 5 && _params.Count != 6)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 5 or 6, found {_params.Count}.");

        return;
      }

      if (!int.TryParse(_params[4], out var quantity) || quantity <= 0)
      {
        SdtdConsole.Instance.Output("Amount is not an integer or not greater than zero.");

        return;
      }

      var itemValue = new ItemValue(ItemClass.GetItem(_params[3], true).type, false);
      if (itemValue.type == ItemValue.None.type)
      {
        SdtdConsole.Instance.Output("Item not found.");

        return;
      }

      if (_params.Count == 6)
      {
        if (!itemValue.HasQuality)
        {
          SdtdConsole.Instance.Output($"Item {_params[3]} does not support quality.");

          return;
        }

        if (!int.TryParse(_params[5], out var quality) || quality <= 0)
        {
          SdtdConsole.Instance.Output("Quality is not an integer or not greater than zero.");

          return;
        }

        itemValue.Quality = quality;
      }

      if (!int.TryParse(_params[0], out var x) ||
          !int.TryParse(_params[1], out var y) ||
          !int.TryParse(_params[2], out var z))
      {
        SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");

        return;
      }

      var itemStack = new ItemStack(itemValue, quantity);
      GameManager.Instance.ItemDropServer(itemStack, new Vector3(x, y, z), Vector3.zero);
      SdtdConsole.Instance.Output("Dropped item");
    }
  }
}
