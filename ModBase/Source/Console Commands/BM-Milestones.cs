using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMMilestones : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Controls Milestones";

    public override string[] GetCommands() => new[] { "bm-milestones" };

    public override string GetHelp() =>
      "Makes adjustments to milestones settings.\n" +
      "Usage:\n" +
      "1. bm-milestones <enable/disable> \n" +
      "2. bm-milestones add \"level\" \"message\" \"item\" \"quantity\" \"quality\"\n" +
      "3. bm-milestones del \"level\"\n" +
      "4. bm-milestones list\n" +
      "   1. enables/disables milestones\n" +
      "   2. adds a milestone\n" +
      "   3. removes a milestone\n" +
      "   4. lists all current milestones\n" +
      " * Include [playername] and/or [lvl] into message to place player info in message\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      switch (_params.Count)
      {
        case 0:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case 1 when _params[0].EqualsCaseInsensitive("enable"):
          LevelSystem.MilestonesEnabled = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ Milestones enabled");
          return;

        case 1 when _params[0].EqualsCaseInsensitive("disable"):
          LevelSystem.ShowLevelEnabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ Milestones disabled");
          return;

        case 1 when _params[0].EqualsCaseInsensitive("list"):
          {
            if (LevelSystem.MilestoneDict.Count == 0)
            {
              SdtdConsole.Instance.Output("~Botman~ There are currently no milestones setup.");

              return;
            }

            SdtdConsole.Instance.Output("~Botman~ Milestones:");
            foreach (var milestone in LevelSystem.MilestoneDict)
            {
              SdtdConsole.Instance.Output($"   Level: {milestone.Key} Message: {milestone.Value}");

              if (LevelSystem.MilestoneRewardDict.Count <= 0) { continue; }

              if (!LevelSystem.MilestoneRewardDict.ContainsKey(milestone.Key)) { continue; }

              var reward = LevelSystem.MilestoneRewardDict[milestone.Key].Split(' ');

              SdtdConsole.Instance.Output(
                $"   Reward Item: {reward[0]} " +
                $"quantity: {(reward.Length > 1 ? reward[1] : "1")} " +
                $"quality: {(reward.Length > 2 ? reward[2] : "1")}");
            }

            return;
          }

        case 1:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case 2 when _params[0].EqualsCaseInsensitive("del"):
          {
            if (!int.TryParse(_params[1], out var level))
            {
              SdtdConsole.Instance.Output("~Botman~ Level must be an integer.");

              return;
            }

            if (!LevelSystem.MilestoneDict.ContainsKey(level))
            {
              SdtdConsole.Instance.Output("~Botman~ Milestones does not contain level " + level + " as a milestone.");

              return;
            }

            LevelSystem.MilestoneDict.Remove(level);
            Config.UpdateXml();
            SdtdConsole.Instance.Output($"~Botman~ Removed level {level} from milestones");

            return;
          }

        default:
          if (_params[0].EqualsCaseInsensitive("add"))
          {
            if (!int.TryParse(_params[1], out var level))
            {
              SdtdConsole.Instance.Output("~Botman~ Level must be an integer.");

              return;
            }

            if (_params[2].Length < 1)
            {
              SdtdConsole.Instance.Output("~Botman~ Unable to detect message in params");

              return;
            }

            if (LevelSystem.MilestoneDict.ContainsKey(level))
            {
              SdtdConsole.Instance.Output($"~Botman~ Milestones already contains level {level} as a milestone.");

              return;
            }

            var reward = "";
            if (_params.Count > 3)
            {
              if (_params[3].Contains(","))
              {
                foreach (var item in _params[3].Split(','))
                {
                  if (ValidItemOrBlock(item)) { continue; }

                  SdtdConsole.Instance.Output($"~Botman~ {item} is not a valid item.");

                  return;
                }
              }
              else if (!ValidItemOrBlock(_params[3]))
              {
                SdtdConsole.Instance.Output($"~Botman~ {_params[3]} is not a valid item.");

                return;
              }

              reward = _params[3];

              if (_params.Count > 4)
              {
                if (!int.TryParse(_params[4], out var quantity))
                {
                  SdtdConsole.Instance.Output("~Botman~ Quantity must be an integer");

                  return;
                }
                reward += $" {quantity}";

                if (_params.Count > 5)
                {
                  if (!int.TryParse(_params[5], out var quality))
                  {
                    SdtdConsole.Instance.Output("~Botman~ Quality must be an integer");

                    return;
                  }

                  if (quality < 0 || quality > 6)
                  {
                    SdtdConsole.Instance.Output("~Botman~ Quality must be an integer between 0 and 6");

                    return;
                  }

                  reward += $" {quality}";
                }
              }

              LevelSystem.MilestoneRewardDict.Add(level, reward);
            }

            LevelSystem.MilestoneDict.Add(level, _params[2]);
            Config.UpdateXml();
            SdtdConsole.Instance.Output($"~Botman~ Added level {level} to milestones with message {_params[2]}. {reward}");

            return;
          }

          SdtdConsole.Instance.Output(GetHelp());
          return;
      }
    }

    public static bool ValidItemOrBlock(string item)
    {
      ItemClass itemClass;
      Block block;
      if (int.TryParse(item, out var id))
      {
        itemClass = ItemClass.GetForId(id);
        block = Block.GetBlockByName(item, true);
      }
      else
      {
        itemClass = ItemClass.GetItemClass(item, true);
        block = Block.GetBlockByName(item, true);
      }

      return itemClass != null || block != null;
    }
  }
}
