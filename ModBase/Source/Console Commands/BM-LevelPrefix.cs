using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMLevelPrefix : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Controls chat level prefixes";

    public override string[] GetCommands() => new[] { "bm-levelprefix" };

    public override string GetHelp() =>
      "Makes adjustments to level prefix from players chat.\n" +
      "Usage:\n" +
      "1. bm-levelprefix <enable/disable> \n" +
      "2. bm-levelprefix color <6 digit hex code> \n" +
      "   1. enables/disables chat prefix\n" +
      "   2. changes color of chat prefix\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count == 1)
      {
        if (_params[0].EqualsCaseInsensitive("enable"))
        {
          LevelSystem.ShowLevelEnabled = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ Chat prefixes will be displayed");

          return;
        }

        if (_params[0].EqualsCaseInsensitive("disable"))
        {
          LevelSystem.ShowLevelEnabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ Chat prefixes will not be displayed");

          return;
        }

        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].EqualsCaseInsensitive("color"))
      {
        if (_params[1].Length < 1 || _params[1].Length > 6)
        {
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        LevelSystem.ShowLevelColor = _params[1];
        Config.UpdateXml();
        SdtdConsole.Instance.Output($"~Botman~ Chat prefixes color has been changed to {LevelSystem.ShowLevelColor}");

        return;
      }

      SdtdConsole.Instance.Output(GetHelp());
    }
  }
}
