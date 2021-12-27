using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSetCustomMessage : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Set custom messages";

    public override string[] GetCommands() => new[] { "bm-setcustommsg" };

    public override string GetHelp() =>
      "Set custom messages\n" +
      "Usage:\n" +
      "1. bm-setcustommsg setnamecolor <option> <color>\n" +
      "2. bm-setcustommsg setmessagecolor <option> <color>\n" +
      "3. bm-setcustommsg setmessage <option> <message>\n" +
      "4. bm-setcustommsg setkillernamecolor <option> <message>\n" +
      "5. bm-setcustommsg setvictimnamecolor <option> <message>\n" +
      "6. bm-setcustommsg set [Enable/Disable]\n" +
      "*<option> must be on of these, login, logout, died or killed\n" +
      "*<color> must be 6 digit hex number\n" +
      "*Use [killer]/[victim] for kill message and [name] for others to place player names.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 2 && _params.Count != 3)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 2 or 3, found {_params.Count}.");

        return;
      }

      var command = _params[0].ToLower();
      var option = _params[1].ToLower();

      if (_params.Count == 2)
      {
        if (command.Equals("set"))
        {
          SetEnabled(option);

          return;
        }

        SdtdConsole.Instance.Output($"Unknown command {command}, or unsupported option {option}");

        return;
      }

      switch (command)
      {
        case "setnamecolor" when option.Equals("login"):
          SetNameColorLogin(_params[2]);
          return;

        case "setnamecolor" when option.Equals("logout"):
          SetNameColorLogout(_params[2]);
          return;

        case "setnamecolor" when option.Equals("died"):
          SetNameColorDied(_params[2]);
          return;

        case "setmessagecolor" when option.Equals("login"):
          SetMessageColorLogin(_params[2]);
          return;

        case "setmessagecolor" when option.Equals("logout"):
          SetMessageColorLogout(_params[2]);
          return;

        case "setmessagecolor" when option.Equals("died"):
          SetMessageColorDied(_params[2]);
          return;

        case "setmessagecolor" when option.Equals("killed"):
          SetMessageColorKilled(_params[2]);
          return;

        case "setmessage" when option.Equals("login"):
          SetMessageLogin(_params[2]);
          return;

        case "setmessage" when option.Equals("logout"):
          SetMessageLogout(_params[2]);
          return;

        case "setmessage" when option.Equals("died"):
          SetMessageDied(_params[2]);
          return;

        case "setmessage" when option.Equals("killed"):
          SetMessageKilled(_params[2]);
          return;

        case "setkillernamecolor" when option.Equals("killed"):
          SetKillerNameColorKilled(_params[2]);
          return;

        case "setvictimnamecolor" when option.Equals("killed"):
          SetVictimNameColorKilled(_params[2]);
          return;

        default:
          SdtdConsole.Instance.Output($"Unknown command {command}, or unsupported option {option}");
          return;
      }
    }

    private static void SetVictimNameColorKilled(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("VictimNameColor set");
      GameMessage.VictimColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetKillerNameColorKilled(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("KillerNameColor set");
      GameMessage.KillerColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetMessageKilled(string message)
    {
      SdtdConsole.Instance.Output("PlayerKilledMessage set");
      GameMessage.PlayerKilled = message;
      Config.UpdateXml();
    }

    private static void SetMessageDied(string message)
    {
      SdtdConsole.Instance.Output("PlayerDiedMessage set");
      GameMessage.PlayerDied = message;
      Config.UpdateXml();
    }

    private static void SetMessageLogout(string message)
    {
      SdtdConsole.Instance.Output("LogOutMessage set");
      GameMessage.LogOutMessage = message;
      Config.UpdateXml();
    }

    private static void SetMessageLogin(string message)
    {
      SdtdConsole.Instance.Output("LogInMessage set");
      GameMessage.LogInMessage = message;
      Config.UpdateXml();
    }

    private static void SetMessageColorKilled(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("PlayerKilledMessageColor set");
      GameMessage.PlayerKilledMessageColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetMessageColorDied(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("PlayerDiedMessageColor set");
      GameMessage.PlayerDiedMessageColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetMessageColorLogout(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("LogOutMessageColor set");
      GameMessage.LogOutMessageColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetMessageColorLogin(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("LoginMessageColor set");
      GameMessage.LoginMessageColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetNameColorDied(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("PlayerDiedNameColor set");
      GameMessage.PlayerDiedNameColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetNameColorLogout(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("LogOutNameColor set");
      GameMessage.LogOutNameColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetNameColorLogin(string _color)
    {
      if (!CmdHelpers.GetColor(_color, out var color)) { return; }

      SdtdConsole.Instance.Output("LogInNameColor set");
      GameMessage.LogInNameColor = $"[{color}]";
      Config.UpdateXml();
    }

    private static void SetEnabled(string option)
    {
      switch (option)
      {
        case "enable":
          GameMessage.Enabled = true;
          break;

        case "disable":
          GameMessage.Enabled = false;
          break;

        default:
          SdtdConsole.Instance.Output("Wrong arguments, command set requires enable or disable as the only options");
          return;
      }

      SdtdConsole.Instance.Output($"CustomMessageFunction {(GameMessage.Enabled ? "enabled" : "disabled")}");
      Config.UpdateXml();
    }
  }
}
