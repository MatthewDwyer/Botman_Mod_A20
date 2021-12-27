using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMLevelAchievements : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Controls Level Achievements";

    public override string[] GetCommands() => new[] { "bm-levelachievement", "bm-la" };

    public override string GetHelp() =>
      "Makes adjustments to level achievement settings\n" +
      "Usage:\n" +
      "1. bm-levelachievement <enable/disable> \n" +
      "2. bm-levelachievement dukes <#> \n" +
      "3. bm-levelachievement maxlvl <#> \n" +
      "   1. enables/disables duke reward when player levels\n" +
      "   2. adjusts amount of dukes a player will recieve when leveling\n" +
      "   3. sets max level player will continue to receive dukes per level\n";

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
          LevelSystem.AwardDukesEnabled = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output($"~Botman~ Player will receive dukes until passing level {LevelSystem.AwardDukesMaxLevel}. Level can be changed with \"bm-levelachievement maxlvl <#>");

          return;
        }

        if (_params[0].EqualsCaseInsensitive("disable"))
        {
          LevelSystem.AwardDukesEnabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ Players will not receive dukes while leveling");

          return;
        }

        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].EqualsCaseInsensitive("dukes"))
      {
        if (_params.Count > 2)
        {
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (!int.TryParse(_params[1], out var amount))
        {
          SdtdConsole.Instance.Output("~Botman~ Amount specified is not a valid integer.");

          return;
        }

        LevelSystem.AwardDukesAmount = amount;
        Config.UpdateXml();
        SdtdConsole.Instance.Output($"~Botman~ Players will receive {amount} dukes per level");

        return;
      }

      if (_params[0].EqualsCaseInsensitive("maxlvl"))
      {
        if (_params.Count > 2)
        {
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (!int.TryParse(_params[1], out var amount))
        {
          SdtdConsole.Instance.Output("~Botman~ Amount specified is not a valid integer.");

          return;
        }

        LevelSystem.AwardDukesMaxLevel = amount;
        Config.UpdateXml();
        SdtdConsole.Instance.Output($"~Botman~ Players will receive {LevelSystem.AwardDukesAmount} dukes per level until passing level {amount}");

        return;
      }

      SdtdConsole.Instance.Output(GetHelp());
    }
  }
}
