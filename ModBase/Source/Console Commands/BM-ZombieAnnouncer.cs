using System.Collections.Generic;
using System.Linq;

namespace Botman.Commands
{
  public class BMZombieAnnouncer : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Adds/Removes/Lists Entities on Zombie Announcer List";

    public override string[] GetCommands() => new[] { "bm-zombieannouncer" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-zombieannouncer list\n" +
      "2. bm-zombieannouncer add (zombiename) (message)\n" +
      "3. bm-zombieannouncer remove (zombiename)\n" +
      "4. bm-zombieannouncer enable/disable\n" +
      "   1. Lists all zombies and their announcement message.\n" +
      "   2. Adds the zombie and message to list\n" +
      "   3. Removes specified zombie and message attached \n" +
      "   4. Enables/Disables zombie announcer\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0])
      {
        case "list":
          ShowList(_params.Count);
          return;

        case "enable":
          SetEnable(_params.Count, true);
          return;

        case "disable":
          SetEnable(_params.Count, false);
          return;

        case "add":
          AddAlert(_params);
          return;

        case "remove":
          RemoveAlert(_params);
          return;

        default:
          SdtdConsole.Instance.Output("Command not recognized. Try bm-zombieannouncer for help.");
          break;
      }
    }

    private static void ShowList(int paramCount)
    {
      if (paramCount > 1)
      {
        SdtdConsole.Instance.Output("Too many arguments, command only accepts list");

        return;
      }

      if (EntityWatch.AlertMessage.Count == 0)
      {
        SdtdConsole.Instance.Output("There are no zombies on the zombie announcement list");

        return;
      }

      SdtdConsole.Instance.Output("Zombie Announcement list:");
      foreach (var message in EntityWatch.AlertMessage)
      {
        SdtdConsole.Instance.Output($"   - Name: \"{message.Key}\" Message: \"{message.Value}\"");
      }
    }

    private static void SetEnable(int paramCount, bool enabled)
    {
      if (paramCount > 1)
      {
        SdtdConsole.Instance.Output("Too many arguments, command must either enable or disable");

        return;
      }

      SdtdConsole.Instance.Output($"Zombie Announcer {(enabled ? "Enabled" : "Disabled")}. Use console command bm-za list, to check current zombies and messages");

      EntityWatch.Enabled = enabled;

      Config.UpdateXml();
    }

    private static void AddAlert(IList<string> _params)
    {
      if (_params.Count < 3)
      {
        SdtdConsole.Instance.Output("Please include the message you would like to trigger");

        return;
      }

      if (EntityWatch.AlertMessage.ContainsKey(_params[1]))
      {
        SdtdConsole.Instance.Output($"Zombie: {_params[1]} is already on the list.");

        return;
      }

      var message = string.Join(" ", _params.Skip(2));
      SdtdConsole.Instance.Output($"Added - Zombie: \"{_params[1]}\" Message: \"{message}\"");

      EntityWatch.AlertMessage.Add(_params[1], message);

      Config.UpdateXml();
    }

    private static void RemoveAlert(IList<string> _params)
    {
      if (_params.Count != 2)
      {
        SdtdConsole.Instance.Output($"Too many arguments. Expected 2 name, found {_params.Count}.");

        return;
      }

      if (!EntityWatch.AlertMessage.ContainsKey(_params[1]))
      {
        SdtdConsole.Instance.Output($"Zombie: {_params[1]} is not on the list.");

        return;
      }

      SdtdConsole.Instance.Output($"Zombie: {_params[1]} has been removed from the list.");

      EntityWatch.AlertMessage.Remove(_params[1]);

      Config.UpdateXml();
    }
  }
}
